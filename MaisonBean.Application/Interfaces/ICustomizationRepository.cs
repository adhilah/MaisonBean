using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface ICustomizationRepository
{
    Task<IEnumerable<BeanType>> GetAllBeanTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<MilkOption>> GetAllMilkOptionsAsync(CancellationToken ct = default);
    Task<BeanType?> GetBeanTypeByIdAsync(Guid id, CancellationToken ct = default);
    Task<MilkOption?> GetMilkOptionByIdAsync(Guid id, CancellationToken ct = default);
}