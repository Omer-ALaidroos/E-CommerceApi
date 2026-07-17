using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class ProductReviewRepository : GenericRepository<ProductReview>, IProductReviewRepository
    {
        private readonly AppDbContext _context;

        public ProductReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductReview?> GetByProductAndUserAsync(int productId, string userId)
        {
            return await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .ToListAsync();
        }

        public async Task<int?> GetPurchasedOrderItemIdAsync(int productId, string userId)
        {
            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId && oi.Order.UserId == userId)
                .Select(oi => (int?)oi.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPurchasedProductAsync(int productId, string userId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }
          
          public async Task<ProductReview?> GetByIdAsync(string reviewId)
        {
            return await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id.ToString() == reviewId);
        }

        public async Task<bool> UpdateReviewApprovalStatusAsync(string reviewId, bool isApproved)
        {
            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id.ToString() == reviewId);

            if (review is null)
            {
                return false;
            }

            review.IsApproved = isApproved;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}