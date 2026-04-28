//using MaisonBean.Application.Interfaces;
//using MaisonBean.Infrastructure.Persistence;

//namespace MaisonBean.Infrastructure;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly AppDbContext _context;

//    public UnitOfWork(AppDbContext context) => _context = context;

//    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
//        _context.SaveChangesAsync(ct);
//}




using MaisonBean.Application.Interfaces;
using MaisonBean.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}