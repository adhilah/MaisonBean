using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _db;

        public WishlistRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId, CancellationToken ct) =>
            await _db.WishlistItems
                .Where(w => w.UserId == userId)
                .ToListAsync(ct);

        public async Task<WishlistItem?> GetByUserAndProductAsync(string userId, int productId, CancellationToken ct) =>
            await _db.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, ct);

        public async Task<WishlistItem?> GetByIdAsync(int id, CancellationToken ct) =>
            await _db.WishlistItems.FindAsync(new object[] { id }, ct);

        public async Task AddAsync(WishlistItem item, CancellationToken ct) =>
            await _db.WishlistItems.AddAsync(item, ct);

        public void Remove(WishlistItem item) =>
            _db.WishlistItems.Remove(item);
    }
}