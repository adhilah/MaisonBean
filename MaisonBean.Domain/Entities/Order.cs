using MaisonBean.Domain.Common;
using MaisonBean.Domain.Enums;

namespace MaisonBean.Domain.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }

    // 🔥 STATUS (Controlled)
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public List<OrderItem> Items { get; set; } = new();

    // 🔥 UPDATE STATUS (Admin flow)
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidTransition(Status, newStatus))
            throw new InvalidOperationException(
                $"Invalid transition: {Status} → {newStatus}");

        Status = newStatus;
        SetUpdatedAt();
    }

    // 🔥 CANCEL ORDER (User)
    public void Cancel()
    {
        if (Status >= OrderStatus.Shipping)
            throw new InvalidOperationException(
                "Cannot cancel after shipping started");

        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }

    // 🔥 STATE MACHINE (CORE LOGIC)
    private bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Pending => next == OrderStatus.Processing,
            OrderStatus.Processing => next == OrderStatus.Shipping,
            OrderStatus.Shipping => next == OrderStatus.OutForDelivery,
            OrderStatus.OutForDelivery => next == OrderStatus.Delivered,
            _ => false
        };
    }
    private List<string> GetNextStatuses(OrderStatus current)
    {
        return current switch
        {
            OrderStatus.Pending => new() { "Processing" },
            OrderStatus.Processing => new() { "Shipping" },
            OrderStatus.Shipping => new() { "OutForDelivery" },
            OrderStatus.OutForDelivery => new() { "Delivered" },
            _ => new List<string>()
        };
    }
}