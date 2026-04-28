using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface IBeanTypeRepository
{
    Task<BeanType?> GetByIdAsync(int id, CancellationToken ct);
}