using MaisonBean.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaisonBean.Application.Products.Queries;

public record GetAllProductsForAdminQuery
    : IRequest<IEnumerable<ProductDto>>;


//handler

public class GetAllProductsForAdminQueryHandler
    : IRequestHandler<
        GetAllProductsForAdminQuery,
        IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;

    public GetAllProductsForAdminQueryHandler(
        IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductDto>> Handle(
        GetAllProductsForAdminQuery request,
        CancellationToken ct)
    {
        var products = await _repo.GetAllForAdminAsync();

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