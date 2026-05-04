using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.MilkOptions.Commands;

public record ToggleMilkOptionCommand(int Id) : IRequest<bool>;

//handler
public class ToggleMilkOptionCommandHandler
    : IRequestHandler<ToggleMilkOptionCommand, bool>
{
    private readonly IMilkOptionRepository _repo;
    private readonly IUnitOfWork _uow;

    public ToggleMilkOptionCommandHandler(IMilkOptionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<bool> Handle(ToggleMilkOptionCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new ArgumentException("Milk option not found");

        entity.ToggleBlock();

        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return entity.IsBlocked;
    }
}