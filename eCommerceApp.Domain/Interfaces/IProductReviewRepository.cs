using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IProductReviewRepository 
    {
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId);
        Task<ProductReview?> GetByProductAndUserAsync(int productId, string userId);
        Task<bool> HasPurchasedProductAsync(int productId, string userId);
        Task<int?> GetPurchasedOrderItemIdAsync(int productId, string userId);
        Task<int> SaveChangesAsync();
          Task<IEnumerable<ProductReview>> GetAllAsync();
          Task AddAsync(ProductReview entity);
        Task UpdateAsync(ProductReview entity);
        Task DeleteAsync(Guid id);
        Task<ProductReview?> GetByIdAsync(string reviewId);
        Task<bool> UpdateReviewApprovalStatusAsync(string reviewId, bool isApproved);
    }
}