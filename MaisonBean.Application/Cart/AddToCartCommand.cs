using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;

public class AddToCartCommand : IRequest<decimal>
{
    [JsonIgnore]
    public string UserId { get; set; } = default!;

    public Guid ProductId { get; set; }
    public bool IsCustomized { get; set; } = false;
    public Guid? BeanId { get; set; }
    public Guid? MilkId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, decimal>
{
    private readonly ICartRepository _cart;
    private readonly IProductRepository _products;
    private readonly IBeanTypeRepository _beans;
    private readonly IMilkOptionRepository _milks;
    private readonly IUnitOfWork _uow;

    public AddToCartCommandHandler(
        ICartRepository cart,
        IProductRepository products,
        IBeanTypeRepository beans,
        IMilkOptionRepository milks,
        IUnitOfWork uow)
    {
        _cart = cart;
        _products = products;
        _beans = beans;
        _milks = milks;
        _uow = uow;
    }

    public async Task<decimal> Handle(AddToCartCommand cmd, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        decimal beanPrice = 0;
        if (cmd.BeanId.HasValue)
        {
            var bean = await _beans.GetByIdAsync(cmd.BeanId.Value, ct)
                ?? throw new KeyNotFoundException("Bean not found.");
            beanPrice = bean.PriceAdd;
        }

        decimal milkPrice = 0;
        if (cmd.MilkId.HasValue)
        {
            var milk = await _milks.GetByIdAsync(cmd.MilkId.Value, ct)
                ?? throw new KeyNotFoundException("Milk not found.");
            milkPrice = milk.PriceAdd;
        }

        decimal unitPrice = product.Price + beanPrice + milkPrice;

        var existing = await _cart.FindExistingAsync(
            cmd.UserId, cmd.ProductId, cmd.IsCustomized, cmd.BeanId, cmd.MilkId, ct);

        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + cmd.Quantity);
            _cart.Update(existing);
        }
        else
        {
            var item = CartItem.Create(
                userId: cmd.UserId,
                productId: cmd.ProductId,
                productName: product.Name,
                productImage: product.Image,
                productCategory: product.Category,
                unitPrice: unitPrice,
                quantity: cmd.Quantity,
                isCustomized: cmd.IsCustomized,
                beanId: cmd.BeanId,
                milkId: cmd.MilkId
            );
            await _cart.AddAsync(item, ct);
        }

        await _uow.SaveChangesAsync(ct);
        return unitPrice;
    }
}