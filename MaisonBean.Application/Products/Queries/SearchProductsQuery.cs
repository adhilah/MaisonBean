using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Queries;

public record SearchProductsQuery(string Term)
    : IRequest<IEnumerable<ProductSearchDto>>;



//handler

public class SearchProductsQueryHandler
    : IRequestHandler<
        SearchProductsQuery,
        IEnumerable<ProductSearchDto>>
{
    private readonly IProductRepository _repo;

    public SearchProductsQueryHandler(
        IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductSearchDto>> Handle(
        SearchProductsQuery request,
        CancellationToken ct)
    {
        var products =
            await _repo.SearchAsync(
                request.Term,
                ct);

        return products.Select(p =>
            new ProductSearchDto
            {
                Name = p.Name,

                Description = p.Description,

                Price = p.Price,

                Category = p.Category,

                Image = p.Image,

                BaseCalories = p.BaseCalories,

                HealthBenefits =
                    p.HealthBenefits
            });
    }
}