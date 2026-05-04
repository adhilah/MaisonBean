using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.Where(p => p.IsActive).ToListAsync();

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Products
            .AnyAsync(p => p.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public void Update(Product product) =>
        _context.Products.Update(product);

    public void Delete(Product product) =>
        _context.Products.Remove(product);
}