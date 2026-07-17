﻿﻿﻿﻿﻿﻿﻿using ECommerce.Core.Entities;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;

using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Entities.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Data
{
   

    public class AppDbContext :  IdentityDbContext<AppUser>, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Achieve> CheckoutAchieves { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
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

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null)
                {
                    foreach (var property in primaryKey.Properties)
                    {
                        if (property.ClrType == typeof(int) || property.ClrType == typeof(long))
                        {
                            property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                        }
                    }
                }
            }

            // Seed payment methods with int values
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod
                {
                    Id = 1,
                    Name = "Credit Card"
                },
                new PaymentMethod
                {
                    Id = 2,
                    Name = "PayPal"
                }
            );

            builder.Entity<PaymentMethod>().Property(p => p.Id).UseIdentityColumn();

            // Configure Order entity
            builder.Entity<Order>(order =>
            {
                // Set the primary key
                order.HasKey(o => o.Id);

                // Configure the TotalAmount to store up to 2 decimal places, which is standard for currency
                order.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

                // Configure the OrderStatus enum to be stored as a string (e.g., "Pending", "Shipped")
                order.Property(o => o.Status).HasConversion<string>();

                // --- Relationships ---

                // One-to-Many: An Order has many OrderItems.
                // If an Order is deleted, its associated OrderItems should also be deleted (Cascade).
                order.HasMany(o => o.OrderItems)
                     .WithOne(oi => oi.Order)
                     .HasForeignKey(oi => oi.OrderId)
                     .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One: Many Orders can have one ShippingAddress.
                // Deleting a ShippingAddress is not allowed if it's linked to an order (Restrict).
               /* order.HasOne<Address>()
                     .WithMany()
                     .HasForeignKey(o => o.ShippingAddressId)
                     .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Many Orders can use one PaymentMethod.
                order.HasOne<PaymentMethod>()
                     .WithMany()
                     .HasForeignKey(o => o.PaymentMethodId)
                     .OnDelete(DeleteBehavior.Restrict);*/
            });

            builder.Entity<OrderItem>(orderItem =>
            {
                orderItem.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
            });

            builder.Entity<CartItem>(cartItem => {
                cartItem.Property(ci => ci.PriceAtTime).HasColumnType("decimal(18,2)");
            });

            builder.Entity<Cart>()
              .HasIndex(c => c.UserId);
            builder.Entity<CartItem>()
                   .HasIndex(ci => new { ci.CartId, ci.ProductId })
                   .IsUnique();

            // Configure AppUser PhoneNumber
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
            });
        }
    }

}
