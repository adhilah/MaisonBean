using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities;

public class MilkOption : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal PriceAdd { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Calories { get; private set; }
    public bool IsBlocked { get; set; }

    private MilkOption() { }

    public static MilkOption Create(string name, decimal priceAdd, string description, int calories)
    {
        return new MilkOption
        {
            Name = name,
            PriceAdd = priceAdd,
            Description = description,
            Calories = calories
        };
    }
    public void ToggleBlock()
    {
        IsBlocked = !IsBlocked;
    }
    public void Update(string name, decimal priceAdd, string description, int calories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (priceAdd < 0)
            throw new ArgumentException("Price cannot be negative");

        if (calories < 0)
            throw new ArgumentException("Calories cannot be negative");

        Name = name;
        PriceAdd = priceAdd;
        Description = description;
        Calories = calories;
    }
}