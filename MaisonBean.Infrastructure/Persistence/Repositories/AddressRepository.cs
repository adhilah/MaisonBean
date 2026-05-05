using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories;
public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<Address?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Addresses.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(Address address, CancellationToken ct)
    {
        await _context.Addresses.AddAsync(address, ct);
    }
}