using MaisonBean.Domain.Enums;

namespace MaisonBean.Application.Orders.Requests;

public class UpdateOrderStatusRequest
{
    public OrderStatus NewStatus { get; set; }
}