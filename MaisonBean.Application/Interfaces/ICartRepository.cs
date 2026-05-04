using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces;

public interface ICartRepository
{
    Task<List<CartItem>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<CartItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CartItem?> FindExistingAsync(
    int userId,
    int productId,
    bool isCustomized,
    int? beanId,
    int? milkId,
    int? strength,
    string? temp,
    int? sweetness,
    CancellationToken ct = default);
    Task AddAsync(CartItem item, CancellationToken ct = default);
    void Update(CartItem item);
    void RemoveItem(CartItem item);
}