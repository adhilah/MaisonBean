using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db) => _db = db;

    public Task<List<CartItem>> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        _db.CartItems
           .Include(c => c.Product)
           .Include(c => c.Bean)
           .Include(c => c.Milk)
           .Where(c => c.UserId == userId)
           .ToListAsync(ct);

    public Task<CartItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.CartItems
           .Include(c => c.Product)
           .Include(c => c.Bean)
           .Include(c => c.Milk)
           .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<CartItem?> FindExistingAsync(
        string userId, Guid productId, bool isCustomized,
        Guid? beanId, Guid? milkId, CancellationToken ct = default) =>
        _db.CartItems.FirstOrDefaultAsync(c =>
            c.UserId == userId &&
            c.ProductId == productId &&
            c.IsCustomized == isCustomized &&
            c.BeanId == beanId &&
            c.MilkId == milkId, ct);

    public async Task AddAsync(CartItem item, CancellationToken ct = default) =>
        await _db.CartItems.AddAsync(item, ct);

    public void Update(CartItem item) => _db.CartItems.Update(item);

    public void RemoveItem(CartItem item) => _db.CartItems.Remove(item);
}