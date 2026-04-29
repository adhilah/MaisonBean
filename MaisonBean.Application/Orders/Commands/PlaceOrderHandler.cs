using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;

namespace MaisonBean.Application.Orders.Commands;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly ICartRepository _cart;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public PlaceOrderHandler(
        ICartRepository cart,
        IOrderRepository orders,
        IUnitOfWork uow)
    {
        _cart = cart;
        _orders = orders;
        _uow = uow;
    }

    public async Task<int> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var cartItems = await _cart.GetByUserIdAsync(cmd.UserId, ct);

        if (!cartItems.Any())
            throw new ArgumentException("Cart is empty");


        decimal subtotal = 0;

        var orderItems = cartItems.Select(c =>
        {
            subtotal += c.UnitPrice * c.Quantity;

            return new OrderItem
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                ProductImage = c.ProductImage,
                ProductCategory = c.ProductCategory,
                //BasePrice = c.UnitPrice,
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity,
                BeanId = c.BeanId,
                MilkId = c.MilkId
            };
        }).ToList();

        decimal shipping = 50;
        decimal total = subtotal + shipping;

        var order = new Order
        {
            UserId = cmd.UserId.ToString(),
            //UserEmail = cmd.UserEmail,
            DeliveryAddress = cmd.DeliveryAddress,
            City = cmd.City,
            Phone = cmd.Phone,
            PaymentMethod = cmd.PaymentMethod,
            UpiId = cmd.UpiId,
            Subtotal = subtotal,
            Shipping = shipping,
            Total = total,
            Status = "pending",
            Items = orderItems
        };

        await _orders.AddAsync(order, ct);

        foreach (var item in cartItems)
            _cart.RemoveItem(item);

        await _uow.SaveChangesAsync(ct);

        return order.Id;
    }
}