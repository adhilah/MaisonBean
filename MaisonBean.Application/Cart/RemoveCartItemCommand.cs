using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Cart;

public record RemoveCartItemCommand(int CartItemId, int UserId) : IRequest<Unit>;

public class RemoveCartItemCommandHandler
    : IRequestHandler<RemoveCartItemCommand, Unit>
{
    private readonly ICartRepository _cart;
    private readonly IUnitOfWork _uow;

    public RemoveCartItemCommandHandler(ICartRepository cart, IUnitOfWork uow)
    {
        _cart = cart;
        _uow = uow;
    }

    public async Task<Unit> Handle(RemoveCartItemCommand cmd, CancellationToken ct)
    {

        if (cmd.UserId <= 0)
            throw new UnauthorizedAccessException("User not authenticated.");

        if (cmd.CartItemId <= 0)
            throw new ArgumentException("Invalid cart item id.");

        var item = await _cart.GetByIdAsync(cmd.CartItemId, ct)
            ?? throw new KeyNotFoundException("Cart item not found.");

        if (item.UserId != cmd.UserId)
            throw new UnauthorizedAccessException("You do not own this cart item.");

        _cart.RemoveItem(item);

        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}