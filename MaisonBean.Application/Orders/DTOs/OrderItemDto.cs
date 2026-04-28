namespace MaisonBean.Application.Orders.DTOs;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public string? ProductCategory { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

    public decimal Total => UnitPrice * Quantity;
}