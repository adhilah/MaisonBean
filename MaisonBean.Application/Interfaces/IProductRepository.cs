using MaisonBean.Domain.Entities;
namespace MaisonBean.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}