using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities;

public class MilkOption : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal PriceAdd { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Calories { get; private set; }

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
}