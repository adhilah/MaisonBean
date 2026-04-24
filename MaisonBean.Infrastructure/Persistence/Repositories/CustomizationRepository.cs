using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories;

public class CustomizationRepository : ICustomizationRepository
{
    private readonly AppDbContext _context;

    public CustomizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BeanType>> GetAllBeanTypesAsync(CancellationToken ct = default)
    {
        return await _context.BeanTypes.ToListAsync(ct);
    }

    public async Task<IEnumerable<MilkOption>> GetAllMilkOptionsAsync(CancellationToken ct = default)
    {
        return await _context.MilkOptions.ToListAsync(ct);
    }

    public async Task<BeanType?> GetBeanTypeByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.BeanTypes.FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<MilkOption?> GetMilkOptionByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.MilkOptions.FirstOrDefaultAsync(m => m.Id == id, ct);
    }
}