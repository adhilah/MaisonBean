// Infrastructure/Repositories/MilkOptionRepository.cs
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Repositories;

public class MilkOptionRepository : IMilkOptionRepository
{
    private readonly AppDbContext _db;
    public MilkOptionRepository(AppDbContext db) => _db = db;

    public Task<MilkOption?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.MilkOptions.FirstOrDefaultAsync(m => m.Id == id, ct);
}