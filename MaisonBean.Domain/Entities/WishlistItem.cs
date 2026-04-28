using MaisonBean.Domain.Entities;

public class WishlistItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; }

    public Product Product { get; set; } = null!;
}