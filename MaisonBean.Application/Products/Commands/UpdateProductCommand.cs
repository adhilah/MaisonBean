using MaisonBean.Application.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Products.Commands;

public class UpdateProductCommand : IRequest<Unit>
{
    public int Id { get; set; }

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

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateProductCommandHandler(IProductRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(UpdateProductCommand req, CancellationToken ct)
    {
        if (req.Id <= 0)
            throw new ArgumentException("Invalid product id.");

        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Product name is required.");

        if (req.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");

        if (req.Stock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        var product = await _repo.GetByIdAsync(req.Id, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        var exists = await _repo.ExistsByNameAsync(req.Name, ct);
        if (exists && !string.Equals(product.Name, req.Name, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Another product with same name already exists.");

        //Update details
        product.UpdateDetails(
            req.Name,
            req.Description,
            req.Price,
            req.Category,
            req.Image,
            req.BaseCalories,
            req.HealthBenefits
        );

        //Update stock
        if (req.Stock > product.StockQuantity)
            product.AddStock(req.Stock - product.StockQuantity);
        else if (req.Stock < product.StockQuantity)
            product.ReduceStock(product.StockQuantity - req.Stock);

        _repo.Update(product);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}