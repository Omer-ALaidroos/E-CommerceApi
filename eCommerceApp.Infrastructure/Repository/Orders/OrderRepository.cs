using ECommerce.Core.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Infrastructure.Data;

namespace eCommerceApp.Infrastructure.Repository.Orders
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
