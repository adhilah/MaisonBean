using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;

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

    public async Task<int> Handle(PlaceSingleOrderCommand cmd, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new Exception("Product not found");

        if (!product.IsActive)
            throw new Exception("Product not available");

        decimal beanPrice = 0;
        decimal milkPrice = 0;

        if (cmd.IsCustomized)
        {
            if (!cmd.BeanId.HasValue || !cmd.MilkId.HasValue)
                throw new Exception("Customization required");

            var bean = await _beans.GetByIdAsync(cmd.BeanId.Value, ct)
                ?? throw new Exception("Invalid Bean");

            var milk = await _milks.GetByIdAsync(cmd.MilkId.Value, ct)
                ?? throw new Exception("Invalid Milk");

            beanPrice = bean.PriceAdd;
            milkPrice = milk.PriceAdd;
        }

        decimal unitPrice = product.Price + beanPrice + milkPrice;
        decimal subtotal = unitPrice * cmd.Quantity;
        decimal shipping = 50;
        decimal total = subtotal + shipping;

        var orderItem = new OrderItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductImage = product.Image,
            ProductCategory = product.Category,
            //BasePrice = product.Price,
            UnitPrice = unitPrice,
            Quantity = cmd.Quantity,
            BeanId = cmd.BeanId,
            MilkId = cmd.MilkId
        };

        var order = new Order
        {
            UserId = cmd.UserId.ToString(),
            DeliveryAddress = cmd.DeliveryAddress,
            City = cmd.City,
            Phone = cmd.Phone,
            PaymentMethod = cmd.PaymentMethod,
            UpiId = cmd.UpiId,
            Subtotal = subtotal,
            Shipping = shipping,
            Total = total,
            //Status = "pending",
            Items = new List<OrderItem> { orderItem }
        };

        await _orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);

        return order.Id;
    }
}