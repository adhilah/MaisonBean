using MediatR;

namespace MaisonBean.Application.Orders.Commands;

public class PlaceOrderCommand : IRequest<int>
{
    public int UserId { get; set; }
    //public string UserEmail { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }
}