using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Repositories;

public class BeanTypeRepository : IBeanTypeRepository
{
    private readonly AppDbContext _db;

    public BeanTypeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BeanType?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.BeanTypes
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<BeanType>> GetAllAsync(CancellationToken ct)
    {
        return await _db.BeanTypes
            .AsNoTracking() // performance optimization
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        return await _db.BeanTypes
            .AnyAsync(b => b.Name == name, ct);
    }

    public async Task AddAsync(BeanType entity, CancellationToken ct)
    {
        await _db.BeanTypes.AddAsync(entity, ct);
    }

    public void Update(BeanType entity)
    {
        _db.BeanTypes.Update(entity);
    }

    public void Delete(BeanType entity)
    {
        _db.BeanTypes.Remove(entity);
    }
}