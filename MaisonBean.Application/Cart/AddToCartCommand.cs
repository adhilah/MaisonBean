using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;

public class AddToCartCommand : IRequest<decimal>
{
    [JsonIgnore]
    public int UserId { get; set; }

    public int ProductId { get; set; }
    public bool IsCustomized { get; set; } = false;

    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

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
        // 🔐 Auth check
        if (cmd.UserId <= 0)
            throw new UnauthorizedAccessException("User not authenticated.");

        // 📦 Quantity validation
        if (cmd.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        // 📦 Product validation
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        if (!product.IsActive)
            throw new InvalidOperationException("Product is not available.");

        if (cmd.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Insufficient stock.");

        // 🔴 FIXED: Customization validation
        if (!cmd.IsCustomized)
        {
            // Ignore customization completely
            cmd.BeanId = null;
            cmd.MilkId = null;
        }
        else
        {
            // 🔴 IMPORTANT: Reject 0 or null
            if (!cmd.BeanId.HasValue || cmd.BeanId <= 0)
                throw new ArgumentException("BeanId is required when customization is enabled.");

            if (!cmd.MilkId.HasValue || cmd.MilkId <= 0)
                throw new ArgumentException("MilkId is required when customization is enabled.");
        }

        // 💰 Pricing
        decimal beanPrice = 0;
        decimal milkPrice = 0;

        if (cmd.IsCustomized)
        {
            // Bean
            var bean = await _beans.GetByIdAsync(cmd.BeanId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid BeanId.");

            beanPrice = bean.PriceAdd;

            // Milk
            var milk = await _milks.GetByIdAsync(cmd.MilkId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid MilkId.");

            milkPrice = milk.PriceAdd;
        }

        decimal unitPrice = product.Price + beanPrice + milkPrice;

        // 🔁 Check existing cart item
        var existing = await _cart.FindExistingAsync(
            cmd.UserId,
            cmd.ProductId,
            cmd.IsCustomized,
            cmd.BeanId,
            cmd.MilkId,
            ct);

        if (existing is not null)
        {
            var newQty = existing.Quantity + cmd.Quantity;

            if (newQty > product.StockQuantity)
                throw new InvalidOperationException("Exceeds available stock.");

            existing.UpdateQuantity(newQty);
            _cart.Update(existing);
        }
        else
        {
            var item = CartItem.Create(
                cmd.UserId,
                cmd.ProductId,
                product.Name,
                product.Image,
                product.Category,
                unitPrice,
                cmd.Quantity,
                cmd.IsCustomized,
                cmd.BeanId,
                cmd.MilkId
            );

            await _cart.AddAsync(item, ct);
        }

        await _uow.SaveChangesAsync(ct);

        return unitPrice;
    }
}