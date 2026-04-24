namespace MaisonBean.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }

    public string UserId { get; private set; } = default!;
    public AppUser User { get; private set; } = default!;

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;

    // Snapshot
    public string ProductName { get; private set; } = default!;
    public string? ProductImage { get; private set; }
    public string? ProductCategory { get; private set; }
    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }
    public bool IsCustomized { get; private set; }

    public Guid? BeanId { get; private set; }
    public BeanType? Bean { get; private set; }

    public Guid? MilkId { get; private set; }
    public MilkOption? Milk { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private CartItem() { }

    public static CartItem Create(
        string userId,
        Guid productId,
        string productName,
        string? productImage,
        string? productCategory,
        decimal unitPrice,
        int quantity,
        bool isCustomized,
        Guid? beanId,
        Guid? milkId)
    {
        return new CartItem
        {
            Id = Guid.NewGuid(),
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

    public void UpdateQuantity(int qty) => Quantity = qty;
}