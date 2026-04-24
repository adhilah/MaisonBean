using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Cart;

public record ClearCartCommand(string UserId) : IRequest<Unit>;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Unit>
{
    private readonly ICartRepository _cart;
    private readonly IUnitOfWork _uow;

    public ClearCartCommandHandler(ICartRepository cart, IUnitOfWork uow)
    {
        _cart = cart;
        _uow = uow;
    }

    public async Task<Unit> Handle(ClearCartCommand cmd, CancellationToken ct)
    {
        var items = await _cart.GetByUserIdAsync(cmd.UserId, ct);
        foreach (var item in items)
            _cart.RemoveItem(item);

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}