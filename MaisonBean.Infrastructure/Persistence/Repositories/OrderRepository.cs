using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId, CancellationToken ct) =>
            await _db.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Date)
                .ToListAsync(ct);

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
            await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task AddAsync(Order order, CancellationToken ct) =>
            await _db.Orders.AddAsync(order, ct);

        public void Update(Order order) =>
            _db.Orders.Update(order);

        public void Remove(Order order) =>
            _db.Orders.Remove(order);
    }
}