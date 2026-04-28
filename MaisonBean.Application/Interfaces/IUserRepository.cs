using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
}