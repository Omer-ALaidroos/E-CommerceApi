using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IProductReviewRepository : IGeneric<ProductReview>
    {
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId);
        Task<ProductReview?> GetByProductAndUserAsync(int productId, string userId);
        Task<bool> HasPurchasedProductAsync(int productId, string userId);
        Task<int?> GetPurchasedOrderItemIdAsync(int productId, string userId);
        Task<int> SaveChangesAsync();
        Task<ProductReview?> GetByIdAsync(string reviewId);
        Task<bool> UpdateReviewApprovalStatusAsync(string reviewId, bool isApproved);
    }
}