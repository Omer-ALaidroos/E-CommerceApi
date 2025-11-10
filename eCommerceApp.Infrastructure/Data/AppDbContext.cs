using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Infrastructure.Data
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Achieve> CheckoutAchieves { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles (IdentityRole.Id is string, so string GUIDs are fine)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "d617eecd-681f-40a3-ae55-7773f85fea20",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "afdc1ce3-488a-4bc3-91b7-7347c3c91ecd",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // If PaymentMethod.Id is a Guid, make sure EF doesn't treat it as generated
            builder.Entity<PaymentMethod>()
                   .Property(pm => pm.Id)
                   .ValueGeneratedNever();

            // Seed payment methods with Guid values (use valid GUID strings)
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod
                {
                    Id = Guid.Parse("d3c9a8e2-1f2b-4a9b-8d7f-1234567890ab"),
                    Name = "Credit Card"
                },
                new PaymentMethod
                {
                    Id = Guid.Parse("b8f4c2a1-2e3d-4f6a-9c8b-abcdef012345"),
                    Name = "PayPal"
                }
            );
        }
    }

}
