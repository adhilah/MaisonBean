using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Queries;

public record GetProductsByCategoryQuery(string Category) : IRequest<IEnumerable<ProductDto>>;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;

    public GetProductsByCategoryQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken ct)
    {
        var products = await _repo.GetByCategoryAsync(request.Category, ct);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            Category = p.Category,
            Image = p.Image,
            BaseCalories = p.BaseCalories,
            HealthBenefits = p.HealthBenefits
        });
    }
}