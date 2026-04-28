using MaisonBean.Application.Interfaces;
using MediatR;

public record ClearCartCommand(int UserId) : IRequest<Unit>;

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
        if (cmd.UserId <= 0)
            throw new ArgumentException("Invalid user.");

        var items = await _cart.GetByUserIdAsync(cmd.UserId, ct);

        foreach (var item in items)
            _cart.RemoveItem(item);

        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}