using MaisonBean.Application.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.BeanTypes.Commands;

public class UpdateBeanTypeCommand : IRequest<Unit>
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public decimal PriceAdd { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}

//handler
public class UpdateBeanTypeCommandHandler
    : IRequestHandler<UpdateBeanTypeCommand, Unit>
{
    private readonly IBeanTypeRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateBeanTypeCommandHandler(IBeanTypeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(UpdateBeanTypeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new KeyNotFoundException("Bean type not found");

        entity.Update(
            request.Name,
            request.PriceAdd,
            request.Description
        );

        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}