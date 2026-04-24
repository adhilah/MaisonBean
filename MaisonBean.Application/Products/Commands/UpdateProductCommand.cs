using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;

namespace MaisonBean.Application.Products.Commands;

public class UpdateProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int BaseCalories { get; set; }
    public string HealthBenefits { get; set; } = string.Empty;
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _uow;

    public UpdateProductCommandHandler(IProductRepository productRepo, IUnitOfWork uow)
    {
        _productRepo = productRepo;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.Id);
        if (product == null) return false;

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Category,
            request.Image,
            request.BaseCalories,
            request.HealthBenefits 
        );

        _productRepo.Update(product);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}