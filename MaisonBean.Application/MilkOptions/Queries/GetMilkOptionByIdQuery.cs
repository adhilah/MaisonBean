using MediatR;
using MaisonBean.Application.MilkOptions.DTOs;
using MaisonBean.Application.Interfaces;

namespace MaisonBean.Application.MilkOptions.Queries;

public record GetMilkOptionByIdQuery(int Id) : IRequest<MilkOptionDto>;


public class GetMilkOptionByIdQueryHandler : IRequestHandler<GetMilkOptionByIdQuery, MilkOptionDto>
{
    private readonly IMilkOptionRepository _repo;

    public GetMilkOptionByIdQueryHandler(IMilkOptionRepository repo)
    {
        _repo = repo;
    }

    public async Task<MilkOptionDto> Handle(GetMilkOptionByIdQuery request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null || entity.IsBlocked) // 🔥 ADD THIS
            throw new ArgumentException("Milk option not found");

        return new MilkOptionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PriceAdd = entity.PriceAdd,
            Description = entity.Description,
            Calories = entity.Calories,
            IsBlocked = entity.IsBlocked
        };
    }
}