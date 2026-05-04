using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.BeanTypes.Commands;

public record ToggleBeanTypeCommand(int Id) : IRequest<bool>;

//handler
public class ToggleBeanTypeCommandHandler
    : IRequestHandler<ToggleBeanTypeCommand, bool>
{
    private readonly IBeanTypeRepository _repo;
    private readonly IUnitOfWork _uow;

    public ToggleBeanTypeCommandHandler(IBeanTypeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<bool> Handle(ToggleBeanTypeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new ArgumentException("Bean type not found");

        entity.ToggleBlock();

        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return entity.IsBlocked;
    }
}