using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MaisonBean.Application.MilkOptions.Commands;

public class UpdateMilkOptionCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal PriceAdd { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Calories { get; set; }
}


public class UpdateMilkOptionCommandHandler
    : IRequestHandler<UpdateMilkOptionCommand, Unit>
{
    private readonly IMilkOptionRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateMilkOptionCommandHandler(IMilkOptionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(UpdateMilkOptionCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null)
            throw new ArgumentException("Milk option not found");

        entity.Update(
            request.Name,
            request.PriceAdd,
            request.Description,
            request.Calories
        );

        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}