using MediatR;
using MaisonBean.Application.MilkOptions.DTOs;
using MaisonBean.Application.Interfaces;

namespace MaisonBean.Application.MilkOptions.Queries;

public record GetMilkOptionsQuery() : IRequest<List<MilkOptionDto>>;

public class GetMilkOptionsQueryHandler : IRequestHandler<GetMilkOptionsQuery, List<MilkOptionDto>>
{
    private readonly IMilkOptionRepository _repo;

    public GetMilkOptionsQueryHandler(IMilkOptionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<MilkOptionDto>> Handle(GetMilkOptionsQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list
            .Where(x => !x.IsBlocked) // 🔥 FILTER HERE
            .Select(x => new MilkOptionDto
            {
                Id = x.Id,
                Name = x.Name,
                PriceAdd = x.PriceAdd,
                Description = x.Description,
                Calories = x.Calories,
                IsBlocked = x.IsBlocked
            })
            .ToList();
    }
}