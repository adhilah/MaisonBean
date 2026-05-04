using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repo;

    public GetProductByIdQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.Id, ct);
        if (product == null || product.IsBlocked)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            Category = product.Category,
            Image = product.Image,
            BaseCalories = product.BaseCalories,
            HealthBenefits = product.HealthBenefits
        };
    }
}