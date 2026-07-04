using ECommerce.Core.Entities;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<AppUser> Users { get; }
        

        DbSet<Category> Categories { get; }

        DbSet<OrderItem> OrderItems { get; }

        DbSet<Product> Products { get; }

        DbSet<Address> Addresses { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);
    }

}
