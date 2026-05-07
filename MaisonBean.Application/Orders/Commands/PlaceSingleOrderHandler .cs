using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;

namespace MaisonBean.Application.Orders.Commands;

public class PlaceSingleOrderHandler
    : IRequestHandler<PlaceSingleOrderCommand, int>
{
    private readonly IProductRepository _products;
    private readonly IBeanTypeRepository _beans;
    private readonly IMilkOptionRepository _milks;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public PlaceSingleOrderHandler(
        IProductRepository products,
        IBeanTypeRepository beans,
        IMilkOptionRepository milks,
        IOrderRepository orders,
        IUnitOfWork uow)
    {
        _products = products;
        _beans = beans;
        _milks = milks;
        _orders = orders;
        _uow = uow;
    }

    public async Task<int> Handle(
        PlaceSingleOrderCommand cmd,
        CancellationToken ct)
    {
        // PRODUCT VALIDATION
        var product = await _products.GetByIdAsync(
            cmd.ProductId,
            ct);

        if (product == null)
            throw new InvalidOperationException(
                "Product not found");

        if (!product.IsActive)
            throw new InvalidOperationException(
                "Product not available");

        if (product.IsBlocked)
            throw new InvalidOperationException(
                "Product is blocked");

        if (cmd.Quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than 0");

        if (product.StockQuantity < cmd.Quantity)
            throw new InvalidOperationException(
                $"Only {product.StockQuantity} items available");

        decimal beanPrice = 0;
        decimal milkPrice = 0;

        BeanType? bean = null;
        MilkOption? milk = null;

        // CUSTOMIZATION VALIDATION
        if (cmd.IsCustomized)
        {
            if (!cmd.BeanId.HasValue)
                throw new InvalidOperationException(
                    "Bean selection required");

            if (!cmd.MilkId.HasValue)
                throw new InvalidOperationException(
                    "Milk selection required");

            bean = await _beans.GetByIdAsync(
                cmd.BeanId.Value,
                ct);

            if (bean == null)
                throw new InvalidOperationException(
                    "Invalid bean");

            milk = await _milks.GetByIdAsync(
                cmd.MilkId.Value,
                ct);

            if (milk == null)
                throw new InvalidOperationException(
                    "Invalid milk");

            beanPrice = bean.PriceAdd;
            milkPrice = milk.PriceAdd;
        }

        // PRICE CALCULATION
        decimal unitPrice =
            product.Price +
            beanPrice +
            milkPrice;

        decimal subtotal =
            unitPrice * cmd.Quantity;

        decimal shipping = 50;

        decimal total =
            subtotal + shipping;

        // ORDER ITEM
        var orderItem = new OrderItem
        {
            ProductId = product.Id,

            ProductName = product.Name,

            ProductImage = product.Image,

            ProductCategory = product.Category,

            UnitPrice = unitPrice,

            Quantity = cmd.Quantity,

            BeanId = cmd.BeanId,

            MilkId = cmd.MilkId,

            BeanPriceAdd = beanPrice,

            MilkPriceAdd = milkPrice
        };

        // ORDER
        var order = new Order
        {
            UserId = cmd.UserId.ToString(),

            AddressId = cmd.AddressId,

            PaymentMethod = cmd.PaymentMethod,

            UpiId = cmd.UpiId,

            Subtotal = subtotal,

            Shipping = shipping,

            Total = total,

            Items = new List<OrderItem>
            {
                orderItem
            }
        };

        // SAVE ORDER
        await _orders.AddAsync(order, ct);

        await _uow.SaveChangesAsync(ct);

        return order.Id;
    }
}