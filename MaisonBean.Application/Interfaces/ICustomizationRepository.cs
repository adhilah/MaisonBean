using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface ICustomizationRepository
{
    Task<IEnumerable<BeanType>> GetAllBeanTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<MilkOption>> GetAllMilkOptionsAsync(CancellationToken ct = default);
    Task<BeanType?> GetBeanTypeByIdAsync(int id, CancellationToken ct = default);
    Task<MilkOption?> GetMilkOptionByIdAsync(int id, CancellationToken ct = default);
}