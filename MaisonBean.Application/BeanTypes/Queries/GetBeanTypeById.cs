using MaisonBean.Application.BeanTypes.DTOs;
using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.BeanTypes.Queries;

public record GetBeanTypeByIdQuery(int Id) : IRequest<BeanTypeDto>;

public class GetBeanTypeByIdQueryHandler : IRequestHandler<GetBeanTypeByIdQuery, BeanTypeDto>
{
    private readonly IBeanTypeRepository _repo;

    public GetBeanTypeByIdQueryHandler(IBeanTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<BeanTypeDto> Handle(GetBeanTypeByIdQuery request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);

        if (entity == null || entity.IsBlocked) // 🔥 FIX
            throw new ArgumentException("Bean type not found.");

        return new BeanTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PriceAdd = entity.PriceAdd,
            Description = entity.Description,
            IsBlocked = entity.IsBlocked
        };
    }
}