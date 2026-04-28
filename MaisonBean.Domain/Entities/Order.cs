using MaisonBean.Domain.Common;

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

    public string Status { get; set; } = "pending";

    public List<OrderItem> Items { get; set; } = new();
}