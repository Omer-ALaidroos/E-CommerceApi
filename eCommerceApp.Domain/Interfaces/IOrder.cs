using eCommerceApp.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore.Storage;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IOrder
    {
        Task<Order?> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllAsync();

        Task AddAsync(Order entity);
        Task UpdateAsync(Order entity);
        Task DeleteAsync(int id);

        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}