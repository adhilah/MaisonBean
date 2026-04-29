using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;

public class AddToCartCommand : IRequest<decimal>, IValidatableObject
{
    [JsonIgnore]
    public int UserId { get; set; }

    [Required(ErrorMessage = "ProductId is required")]
    public int ProductId { get; set; }

    public bool IsCustomized { get; set; }

    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

    // ✅ Default quantity = 1
    public int Quantity { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsCustomized)
        {
            if (BeanId.HasValue || MilkId.HasValue)
            {
                yield return new ValidationResult(
                    "BeanId and MilkId must be null when IsCustomized is false",
                    new[] { nameof(BeanId), nameof(MilkId) });
            }
        }

        if (IsCustomized)
        {
            if (!BeanId.HasValue || BeanId <= 0)
            {
                yield return new ValidationResult(
                    "BeanId is required when customization is enabled",
                    new[] { nameof(BeanId) });
            }

            if (!MilkId.HasValue || MilkId <= 0)
            {
                yield return new ValidationResult(
                    "MilkId is required when customization is enabled",
                    new[] { nameof(MilkId) });
            }
        }
    }
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

        // ✅ FIX: Quantity default handling
        if (cmd.Quantity <= 0)
            cmd.Quantity = 1;

        // 🛑 Optional limit
        if (cmd.Quantity > 10)
            throw new ArgumentException("Maximum quantity allowed is 10");

        // 📦 Product validation
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        if (!product.IsActive)
            throw new InvalidOperationException("Product is not available.");

        if (cmd.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Insufficient stock.");

        // 🎯 Customization handling
        if (!cmd.IsCustomized)
        {
            cmd.BeanId = null;
            cmd.MilkId = null;
        }
        else
        {
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
            var bean = await _beans.GetByIdAsync(cmd.BeanId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid BeanId.");

            var milk = await _milks.GetByIdAsync(cmd.MilkId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid MilkId.");

            beanPrice = bean.PriceAdd;
            milkPrice = milk.PriceAdd;
        }

        decimal unitPrice = product.Price + beanPrice + milkPrice;

        // ✅ TOTAL PRICE
        decimal totalPrice = unitPrice * cmd.Quantity;

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

        
        return totalPrice;
    }
}