
using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Interfaces.CartInterface;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository.CartRepo
{
	public class CartRepository(AppDbContext context) : ICart
	{
		public async Task<IEnumerable<Achieve>> GetAllCheckoutHistory()
		{
			return await context.CheckoutAchieves.AsNoTracking().ToListAsync();
		}

		public async Task<int> SaveCheckoutHistory(IEnumerable<Achieve> checkOuts)
		{
			await context.CheckoutAchieves.AddRangeAsync(checkOuts);
			return await context.SaveChangesAsync();
		}

		public async Task<Cart?> GetActiveCart(string userId)
		{
			return await context.Carts
				.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);
		}

		public async Task<Cart> CreateCart(Cart cart)
		{
			await context.Carts.AddAsync(cart);
			return cart;
		}

		public async Task AddCartItem(CartItem item)
		{
			await context.CartItems.AddAsync(item);
		}

		public Task UpdateCartItem(CartItem item)
		{
			context.CartItems.Update(item);
			return Task.CompletedTask;
		}

		public async Task SaveChanges()
		{
			await context.SaveChangesAsync();
		}
	}
}
