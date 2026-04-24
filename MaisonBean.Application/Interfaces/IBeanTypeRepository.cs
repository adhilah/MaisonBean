using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IBeanTypeRepository
{
    Task<BeanType?> GetByIdAsync(Guid id, CancellationToken ct);
}