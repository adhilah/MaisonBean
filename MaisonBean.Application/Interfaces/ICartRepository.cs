using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface ICartRepository
{
    Task<List<CartItem>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<CartItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CartItem?> FindExistingAsync(string userId, Guid productId, bool isCustomized, Guid? beanId, Guid? milkId, CancellationToken ct = default);
    Task AddAsync(CartItem item, CancellationToken ct = default);
    void Update(CartItem item);
    void RemoveItem(CartItem item);
}