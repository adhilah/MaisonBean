using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Wishlist.DTOs;
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
        public async Task<List<WishlistItemDto>> GetWishlistWithProducts(string userId, CancellationToken ct)
        {
            return await _db.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Select(w => new WishlistItemDto
                {
                    WishlistId = w.Id,
                    ProductId = w.Product.Id,
                    Name = w.Product.Name,
                    Price = w.Product.Price,
                    Image = w.Product.Image,
                    Description = w.Product.Description,
                    Category = w.Product.Category,
                    HealthBenefits = w.Product.HealthBenefits,
                    BaseCalories = w.Product.BaseCalories
                })
                .ToListAsync(ct);
        }
    }
}