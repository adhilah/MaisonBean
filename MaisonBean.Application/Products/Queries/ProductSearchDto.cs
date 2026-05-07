namespace MaisonBean.Application.Products.Queries;

public class ProductSearchDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? Image { get; set; }

    public int BaseCalories { get; set; }

    public string? HealthBenefits { get; set; }
}