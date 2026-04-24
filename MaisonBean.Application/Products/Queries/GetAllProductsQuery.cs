using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;

    public GetAllProductsQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var products = await _repo.GetAllAsync();

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