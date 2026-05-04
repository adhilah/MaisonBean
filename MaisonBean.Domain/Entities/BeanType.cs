namespace MaisonBean.Domain.Entities;

public class BeanType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal PriceAdd { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool IsBlocked { get; set; }

    private BeanType() { }

    public static BeanType Create(string name, decimal priceAdd, string description) =>
        new() { Name = name, PriceAdd = priceAdd, Description = description };

    public void Update(string name, decimal priceAdd, string description)
    {
        Name = name;
        PriceAdd = priceAdd;
        Description = description;
    }

    public void ToggleBlock()
    {
        IsBlocked = !IsBlocked;
    }
}