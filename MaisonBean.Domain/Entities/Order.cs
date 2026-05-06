using MaisonBean.Domain.Common;
using MaisonBean.Domain.Entities;
using MaisonBean.Domain.Enums;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public int? AddressId { get; set; }
    public Address? Address { get; set; } = null!;

    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }

    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public DateTime? PaidAt { get; set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public List<OrderItem> Items { get; set; } = new();

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidTransition(Status, newStatus))
            throw new InvalidOperationException(
                $"Invalid transition: {Status} → {newStatus}");

        Status = newStatus;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status >= OrderStatus.Shipping)
            throw new InvalidOperationException(
                "Cannot cancel after shipping started");

        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }

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
}