namespace MaisonBean.Application.Wishlist.DTOs;
public class WishlistItemDto
{
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string HealthBenefits { get; set; } = string.Empty;
    public int BaseCalories { get; set; }
}