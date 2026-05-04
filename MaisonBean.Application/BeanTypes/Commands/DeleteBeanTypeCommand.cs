using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.BeanTypes.Commands;

public record DeleteBeanTypeCommand(int Id) : IRequest<Unit>;

//handler
public class DeleteBeanTypeCommandHandler
    : IRequestHandler<DeleteBeanTypeCommand, Unit>
{
    private readonly IBeanTypeRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteBeanTypeCommandHandler(IBeanTypeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteBeanTypeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new KeyNotFoundException("Bean type not found");

        _repo.Delete(entity);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}