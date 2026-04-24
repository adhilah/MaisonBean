using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId, CancellationToken ct);
        Task<WishlistItem?> GetByUserAndProductAsync(string userId, Guid productId, CancellationToken ct);
        Task<WishlistItem?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(WishlistItem item, CancellationToken ct);
        void Remove(WishlistItem item);
    }
}