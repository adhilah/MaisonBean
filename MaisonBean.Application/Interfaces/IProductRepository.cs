using MaisonBean.Domain.Entities;
namespace MaisonBean.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Product>> SearchAsync(string term,CancellationToken ct = default);
    Task<IEnumerable<Product>> GetAllForAdminAsync();
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}