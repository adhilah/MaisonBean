using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;

namespace MaisonBean.Application.Products.Commands;

public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int BaseCalories { get; set; }
    public string HealthBenefits { get; set; } = string.Empty;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _uow;

    public CreateProductCommandHandler(IProductRepository productRepo, IUnitOfWork uow)
    {
        _productRepo = productRepo;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.Category,
            request.Image,
            request.BaseCalories,
            request.HealthBenefits
        );

        await _productRepo.AddAsync(product);
        await _uow.SaveChangesAsync(ct);
        return product.Id;
    }
}