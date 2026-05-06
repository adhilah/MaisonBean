using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;
public interface IAddressRepository
{
    Task<List<Address>> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<Address?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Address address, CancellationToken ct);
    void Update(Address address);
}