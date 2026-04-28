using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Orders.DTOs;

public static class OrderMapper
{
    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Subtotal = order.Subtotal,
            Shipping = order.Shipping,
            Total = order.Total,
            Status = order.Status,

            DeliveryAddress = order.DeliveryAddress,
            City = order.City,
            Phone = order.Phone,

            PaymentMethod = order.PaymentMethod,
            CreatedAt = order.CreatedAt,

            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                ProductCategory = i.ProductCategory,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                BeanId = i.BeanId,
                MilkId = i.MilkId
            }).ToList()
        };
    }

    public static List<OrderDto> ToDtoList(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto).ToList();
    }
}