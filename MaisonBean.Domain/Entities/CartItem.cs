namespace MaisonBean.Domain.Entities;

public class CartItem
{
    public int Id { get; private set; }

    public int UserId { get; private set; } = default!;
    public AppUser User { get; private set; } = default!;

    public int ProductId { get; private set; }
    public Product Product { get; private set; } = default!;

    // Snapshot
    public string ProductName { get; private set; } = default!;
    public string? ProductImage { get; private set; }
    public string? ProductCategory { get; private set; }
    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }
    public bool IsCustomized { get; private set; }

    public int? BeanId { get; private set; }
    public BeanType? Bean { get; private set; }

    public int? MilkId { get; private set; }
    public MilkOption? Milk { get; private set; }

    public int? Strength { get; private set; }
    public string? Temp { get; private set; }
    public int? Sweetness { get; private set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    private CartItem() { }

    public static CartItem Create(
        int userId,
        int productId,
        string productName,
        string? productImage,
        string? productCategory,
        decimal unitPrice,
        int quantity,
        bool isCustomized,
        int? beanId,
        int? milkId)
    {
        return new CartItem
        {
            UserId = userId,
            ProductId = productId,
            ProductName = productName,
            ProductImage = productImage,
            ProductCategory = productCategory,
            UnitPrice = unitPrice,
            Quantity = quantity,
            IsCustomized = isCustomized,
            BeanId = beanId,
            MilkId = milkId,
        };
    }
    public void SetCustomization(int? strength, string? temp, int? sweetness)
    {
        Strength = strength;
        Temp = temp;
        Sweetness = sweetness;
    }

    public void UpdateQuantity(int qty) => Quantity = qty;
}