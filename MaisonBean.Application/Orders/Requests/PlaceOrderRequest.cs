namespace MaisonBean.Application.Orders.Requests;

public class PlaceOrderRequest
{
    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }
}