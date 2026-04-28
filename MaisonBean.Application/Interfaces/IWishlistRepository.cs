using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId, CancellationToken ct);
        Task<WishlistItem?> GetByUserAndProductAsync(string userId, int productId, CancellationToken ct);
        Task<WishlistItem?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(WishlistItem item, CancellationToken ct);
        void Remove(WishlistItem item);
    }
}