using eCommerceApp.Domain.Entities.Orders;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eCommerceApp.Infrastructure.Repository.Orders
{
    public class OrderRepository(AppDbContext context) : IOrder
    {
        private readonly AppDbContext _context = context;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task AddAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Orders.FindAsync(id);

            if (entity == null)
                throw new InvalidOperationException("Order not found.");

            _context.Orders.Remove(entity);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userId);
        }

        public Task UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);

            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}