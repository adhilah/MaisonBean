using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.BeanTypes.Commands;

public class CreateBeanTypeCommand : IRequest<int>
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal PriceAdd { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}


//handler
public class CreateBeanTypeCommandHandler
    : IRequestHandler<CreateBeanTypeCommand, int>
{
    private readonly IBeanTypeRepository _beanRepo;
    private readonly IUnitOfWork _uow;

    public CreateBeanTypeCommandHandler(IBeanTypeRepository beanRepo, IUnitOfWork uow)
    {
        _beanRepo = beanRepo;
        _uow = uow;
    }

    public async Task<int> Handle(CreateBeanTypeCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Bean type name is required.");

        if (request.PriceAdd < 0)
            throw new ArgumentException("Price cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");

        var exists = await _beanRepo.ExistsByNameAsync(request.Name, ct);
        if (exists)
            throw new ArgumentException("Bean type already exists.");

        var bean = BeanType.Create(
            request.Name,
            request.PriceAdd,
            request.Description
        );

        await _beanRepo.AddAsync(bean, ct);
        await _uow.SaveChangesAsync(ct);

        return bean.Id;
    }
}