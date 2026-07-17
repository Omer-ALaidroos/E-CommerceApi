using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IProductReviewService
    {
        Task<IEnumerable<ProductReviewDto>> GetReviewsForProductAsync(int productId);
        Task<ServicesResponse> AddReviewAsync(string userId, CreateProductReviewDto dto);
        Task<CanReviewResponseDto> CanUserReviewProductAsync(string userId, int productId);
        Task<ServicesResponse> HideReviewAsync(string reviewId);
        Task<ServicesResponse> ShowReviewAsync(string reviewId);
    }
}
