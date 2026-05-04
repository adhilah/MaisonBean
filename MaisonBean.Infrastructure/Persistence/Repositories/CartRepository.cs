using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Include(c => c.Bean)
            .Include(c => c.Milk)
            .Where(c => c.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<CartItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    // 🔥 FIXED METHOD
    public async Task<CartItem?> FindExistingAsync(
        int userId,
        int productId,
        bool isCustomized,
        int? beanId,
        int? milkId,
        int? strength,
        string? temp,
        int? sweetness,
        CancellationToken ct = default)
    {
        return await _context.CartItems.FirstOrDefaultAsync(c =>
            c.UserId == userId &&
            c.ProductId == productId &&
            c.IsCustomized == isCustomized &&
            c.BeanId == beanId &&
            c.MilkId == milkId &&
            c.Strength == strength &&        // 🔥 IMPORTANT
            c.Temp == temp &&                // 🔥 IMPORTANT
            c.Sweetness == sweetness,        // 🔥 IMPORTANT
            ct);
    }

    public async Task AddAsync(CartItem item, CancellationToken ct = default)
    {
        await _context.CartItems.AddAsync(item, ct);
    }

    public void Update(CartItem item)
    {
        _context.CartItems.Update(item);
    }

    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
    }
}