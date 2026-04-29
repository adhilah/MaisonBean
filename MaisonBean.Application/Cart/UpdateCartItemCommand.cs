using MaisonBean.Application.Interfaces;
using MediatR;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;


public class UpdateCartItemCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int UserId { get; set; }

    public int CartItemId { get; set; }

    public int Quantity { get; set; }
}
public class UpdateCartItemCommandHandler
    : IRequestHandler<UpdateCartItemCommand, Unit>
{
    private readonly ICartRepository _cart;
    private readonly IUnitOfWork _uow;

    public UpdateCartItemCommandHandler(
        ICartRepository cart,
        IUnitOfWork uow)
    {
        _cart = cart;
        _uow = uow;
    }

    public async Task<Unit> Handle(UpdateCartItemCommand cmd, CancellationToken ct)
    {
        // 🔐 Auth check
        if (cmd.UserId <= 0)
            throw new UnauthorizedAccessException("User not authenticated.");

        // 🔍 Get cart item FIRST
        var item = await _cart.GetByIdAsync(cmd.CartItemId, ct)
            ?? throw new KeyNotFoundException("Cart item not found.");

        // ✅ NOW log values
        Console.WriteLine($"Token UserId: {cmd.UserId}");
        Console.WriteLine($"CartItem UserId: {item.UserId}");

        // 🔐 Ownership check
        if (item.UserId != cmd.UserId)
            throw new UnauthorizedAccessException("You cannot modify this item.");

        // 🔥 Quantity logic
        if (cmd.Quantity <= 0)
        {
            _cart.RemoveItem(item);
        }
        else
        {
            if (cmd.Quantity > 10)
                throw new ArgumentException("Maximum quantity allowed is 10");

            item.UpdateQuantity(cmd.Quantity);
            _cart.Update(item);
        }

        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}