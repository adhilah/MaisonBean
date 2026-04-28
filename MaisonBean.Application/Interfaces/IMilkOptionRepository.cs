using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IMilkOptionRepository
{
    Task<MilkOption?> GetByIdAsync(int id, CancellationToken ct);
}