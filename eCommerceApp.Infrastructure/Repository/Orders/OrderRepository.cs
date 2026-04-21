using ECommerce.Core.Entities;
using eCommerceApp.Infrastructure.Data;

namespace eCommerceApp.Infrastructure.Repository.Orders
{
    // Inherit from GenericRepository<Order> and implement IOrderService
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

       
    }
}