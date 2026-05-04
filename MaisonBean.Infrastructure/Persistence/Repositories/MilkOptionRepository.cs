using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Repositories;

public class MilkOptionRepository : IMilkOptionRepository
{
    private readonly AppDbContext _db;

    public MilkOptionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MilkOption?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.MilkOptions
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<MilkOption>> GetAllAsync(CancellationToken ct)
    {
        return await _db.MilkOptions
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        return await _db.MilkOptions
            .AnyAsync(m => m.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task AddAsync(MilkOption entity, CancellationToken ct)
    {
        await _db.MilkOptions.AddAsync(entity, ct);
    }

    public void Update(MilkOption entity)
    {
        _db.MilkOptions.Update(entity);
    }

    public void Delete(MilkOption entity)
    {
        _db.MilkOptions.Remove(entity);
    }
}