using MaisonBean.Application.BeanTypes.DTOs;
using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.BeanTypes.Queries;

public record GetBeanTypesQuery() : IRequest<List<BeanTypeDto>>;

public class GetBeanTypesQueryHandler : IRequestHandler<GetBeanTypesQuery, List<BeanTypeDto>>
{
    private readonly IBeanTypeRepository _repo;

    public GetBeanTypesQueryHandler(IBeanTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BeanTypeDto>> Handle(GetBeanTypesQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list
            .Where(x => !x.IsBlocked) // 🔥 FILTER
            .Select(x => new BeanTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                PriceAdd = x.PriceAdd,
                Description = x.Description,
                IsBlocked = x.IsBlocked
            })
            .ToList();
    }
}