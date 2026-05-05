using MediatR;

namespace MaisonBean.Application.Orders.Commands;

public class PlaceOrderCommand : IRequest<int>
{
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public string PaymentMethod { get; set; }
    public string? UpiId { get; set; }
}