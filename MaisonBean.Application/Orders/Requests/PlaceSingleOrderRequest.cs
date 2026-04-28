namespace MaisonBean.Application.Orders.Requests;

public class PlaceSingleOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public bool IsCustomized { get; set; }
    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }
}