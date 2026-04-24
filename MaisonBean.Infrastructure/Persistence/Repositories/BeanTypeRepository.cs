// Infrastructure/Repositories/BeanTypeRepository.cs
using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Repositories;

public class BeanTypeRepository : IBeanTypeRepository
{
    private readonly AppDbContext _db;
    public BeanTypeRepository(AppDbContext db) => _db = db;

    public Task<BeanType?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.BeanTypes.FirstOrDefaultAsync(b => b.Id == id, ct);
}