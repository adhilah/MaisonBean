using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.MilkOptions.Commands;

public record DeleteMilkOptionCommand(int Id) : IRequest<Unit>;

public class DeleteMilkOptionCommandHandler
    : IRequestHandler<DeleteMilkOptionCommand, Unit>
{
    private readonly IMilkOptionRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteMilkOptionCommandHandler(IMilkOptionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteMilkOptionCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new KeyNotFoundException("Milk option not found");

        _repo.Delete(entity);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}