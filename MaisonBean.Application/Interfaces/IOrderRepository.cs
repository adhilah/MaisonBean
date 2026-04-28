using MaisonBean.Domain.Entities;

namespace MaisonBean.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId, CancellationToken ct);
        Task<Order?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(Order order, CancellationToken ct);
        void Update(Order order);
        void Remove(Order order);
    }
}