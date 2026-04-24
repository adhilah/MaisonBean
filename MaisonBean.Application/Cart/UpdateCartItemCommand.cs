using MaisonBean.Application.Interfaces;
using MediatR;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.Cart;

public class UpdateCartItemCommand : IRequest<Unit>
{
    [JsonIgnore] public Guid CartItemId { get; set; }
    [JsonIgnore] public string UserId { get; set; } = default!;
    public int Quantity { get; set; }
}

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Unit>
{
    private readonly ICartRepository _cart;
    private readonly IUnitOfWork _uow;

    public UpdateCartItemCommandHandler(ICartRepository cart, IUnitOfWork uow)
    {
        _cart = cart;
        _uow = uow;
    }

    public async Task<Unit> Handle(UpdateCartItemCommand cmd, CancellationToken ct)
    {
        var item = await _cart.GetByIdAsync(cmd.CartItemId, ct)
            ?? throw new KeyNotFoundException("Cart item not found.");

        if (item.UserId != cmd.UserId)
            throw new UnauthorizedAccessException("You do not own this cart item.");

        if (cmd.Quantity <= 0)
            _cart.RemoveItem(item);
        else
        {
            item.UpdateQuantity(cmd.Quantity);
            _cart.Update(item);
        }

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}