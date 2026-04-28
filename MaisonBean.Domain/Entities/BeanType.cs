namespace MaisonBean.Domain.Entities;

public class BeanType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal PriceAdd { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private BeanType() { }

    public static BeanType Create(string name, decimal priceAdd, string description) =>
        new() { Name = name, PriceAdd = priceAdd, Description = description };
}