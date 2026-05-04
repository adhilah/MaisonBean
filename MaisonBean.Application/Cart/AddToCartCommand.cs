using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;
public class AddToCartResult
{
    public int CartItemId { get; set; }
    public decimal Total { get; set; }
}
public class AddToCartCommand : IRequest<AddToCartResult>, IValidatableObject
{
    [JsonIgnore]
    public int UserId { get; set; }

    [Required(ErrorMessage = "ProductId is required")]
    public int ProductId { get; set; }

    public bool IsCustomized { get; set; }

    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

    public int Quantity { get; set; } = 1;
    public int? Strength { get; set; }
    public string? Temp { get; set; }
    public int? Sweetness { get; set; }
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

public class AddToCartCommandHandler
    : IRequestHandler<AddToCartCommand, AddToCartResult>
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

    public async Task<AddToCartResult> Handle(AddToCartCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId <= 0)
            throw new UnauthorizedAccessException("User not authenticated.");

        if (cmd.Quantity <= 0)
            cmd.Quantity = 1;

        if (cmd.Quantity > 10)
            throw new ArgumentException("Max quantity is 10");

        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found");

        if (!product.IsActive)
            throw new InvalidOperationException("Product not available");

        if (cmd.Quantity > product.StockQuantity)
            throw new InvalidOperationException("Insufficient stock");

        // 🔥 Customization handling
        decimal beanPrice = 0;
        decimal milkPrice = 0;

        if (!cmd.IsCustomized)
        {
            cmd.BeanId = null;
            cmd.MilkId = null;
        }
        else
        {
            var bean = await _beans.GetByIdAsync(cmd.BeanId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid bean");

            var milk = await _milks.GetByIdAsync(cmd.MilkId!.Value, ct)
                ?? throw new KeyNotFoundException("Invalid milk");

            // 🚨 IMPORTANT: prevent blocked options
            if (bean.IsBlocked)
                throw new InvalidOperationException("Bean is not available");

            if (milk.IsBlocked)
                throw new InvalidOperationException("Milk is not available");

            beanPrice = bean.PriceAdd;
            milkPrice = milk.PriceAdd;
        }

        decimal unitPrice = product.Price + beanPrice + milkPrice;

        // 🔥 UPDATED: include customization fields
        var existing = await _cart.FindExistingAsync(
            cmd.UserId,
            cmd.ProductId,
            cmd.IsCustomized,
            cmd.BeanId,
            cmd.MilkId,
            cmd.Strength,
            cmd.Temp,
            cmd.Sweetness,
            ct
        );

        int cartItemId;

        if (existing != null)
        {
            var newQty = existing.Quantity + cmd.Quantity;

            if (newQty > product.StockQuantity)
                throw new InvalidOperationException("Exceeds stock");

            existing.UpdateQuantity(newQty);
            _cart.Update(existing);
            await _uow.SaveChangesAsync(ct);

            cartItemId = existing.Id;
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

            // 🔥 NEW (store customization)
            item.SetCustomization(cmd.Strength, cmd.Temp, cmd.Sweetness);

            await _cart.AddAsync(item, ct);
            await _uow.SaveChangesAsync(ct);

            cartItemId = item.Id;
        }

        return new AddToCartResult
        {
            CartItemId = cartItemId,
            Total = unitPrice * cmd.Quantity
        };
    }
}