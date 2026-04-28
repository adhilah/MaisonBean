using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Products.Commands;

public class CreateProductCommand : IRequest<int>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    [Required]
    public string Category { get; set; } = string.Empty;
    [Required]
    public string Image { get; set; } = string.Empty;
    [Range(0, int.MaxValue)]
    public int BaseCalories { get; set; }
    [Required]
    public string HealthBenefits { get; set; } = string.Empty;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _uow;

    public CreateProductCommandHandler(IProductRepository productRepo, IUnitOfWork uow)
    {
        _productRepo = productRepo;
        _uow = uow;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        //Validations
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");

        if (request.Stock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Category is required.");

        if (request.BaseCalories < 0)
            throw new ArgumentException("Calories cannot be negative.");

        // (Optional) check duplicate product
        var exists = await _productRepo.ExistsByNameAsync(request.Name, ct);
        if (exists)
            throw new ArgumentException("Product with same name already exists.");

 
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