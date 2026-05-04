using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.MilkOptions.Commands;

public class CreateMilkOptionCommand : IRequest<int>
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal PriceAdd { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Calories { get; set; }
}


public class CreateMilkOptionCommandHandler : IRequestHandler<CreateMilkOptionCommand, int>
{
    private readonly IMilkOptionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateMilkOptionCommandHandler(IMilkOptionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<int> Handle(CreateMilkOptionCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required");

        if (request.PriceAdd < 0)
            throw new ArgumentException("Price cannot be negative");

        if (request.Calories < 0)
            throw new ArgumentException("Calories cannot be negative");

        var exists = await _repo.ExistsByNameAsync(request.Name, ct);
        if (exists)
            throw new ArgumentException("Milk option already exists");

        var entity = MilkOption.Create(
            request.Name,
            request.PriceAdd,
            request.Description,
            request.Calories
        );

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return entity.Id;
       
    }
}