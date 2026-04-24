using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string Category { get; private set; } = string.Empty;
    public string Image { get; private set; } = string.Empty;
    public int BaseCalories { get; private set; }
    public string HealthBenefits { get; private set; } = string.Empty;

    private Product() { }

    public static Product Create(
        string name, string description, decimal price, int stock,
        string category, string image, int baseCalories, string healthBenefits)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stock,
            Category = category,
            Image = image,
            BaseCalories = baseCalories,
            HealthBenefits = healthBenefits
        };
    }

    // ── Update method ──
    public void Update(
        string name, string description, decimal price,
        string category, string image,
        int baseCalories, string healthBenefits)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        Name = name;
        Description = description;
        Price = price;
        Category = category;
        Image = image;
        BaseCalories = baseCalories;
        HealthBenefits = healthBenefits;
        SetUpdatedAt();
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException("Insufficient stock.");
        StockQuantity -= quantity;
        SetUpdatedAt();
    }

    public void AddStock(int quantity)
    {
        StockQuantity += quantity;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}