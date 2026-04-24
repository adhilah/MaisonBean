using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IMilkOptionRepository
{
    Task<MilkOption?> GetByIdAsync(Guid id, CancellationToken ct);
}