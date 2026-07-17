using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementation
{
    public class ProductReviewService(IProductReviewRepository reviewRepository, IMapper mapper, IProduct productRepository) : IProductReviewService
    {
        public async Task<IEnumerable<ProductReviewDto>> GetReviewsForProductAsync(int productId)
        {
            var reviews = await reviewRepository.GetByProductIdAsync(productId);
            return mapper.Map<IEnumerable<ProductReviewDto>>(reviews);
        }
        public async Task<CanReviewResponseDto> CanUserReviewProductAsync(string userId, int productId)
        {
            var hasPurchased = await reviewRepository.HasPurchasedProductAsync(productId, userId);
            var alreadyReviewed = await reviewRepository.GetByProductAndUserAsync(productId, userId) != null;

            return new CanReviewResponseDto
            {
                HasPurchased = hasPurchased,
                AlreadyReviewed = alreadyReviewed,
                CanReview = hasPurchased && !alreadyReviewed
            };
        }


        public async Task<ServicesResponse> AddReviewAsync(string userId, CreateProductReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return new ServicesResponse(false, "Rating must be between 1 and 5.");
            }

            var existingReview = await reviewRepository.GetByProductAndUserAsync(dto.ProductId, userId);
            if (existingReview is not null)
            {
                return new ServicesResponse(false, "You have already reviewed this product.");
            }

            var canReview = await reviewRepository.HasPurchasedProductAsync(dto.ProductId, userId);
            if (!canReview)
            {
                return new ServicesResponse(false, "Only users who purchased this product can review it.");
            }

            var orderItemId = await reviewRepository.GetPurchasedOrderItemIdAsync(dto.ProductId, userId);
            if (orderItemId is null)
            {
                return new ServicesResponse(false, "We could not find a valid purchased item for this review.");
            }

            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = userId,
                OrderItemId = orderItemId.Value,
                Rating = dto.Rating,
                Review = dto.Review,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };

            await reviewRepository.AddAsync(review);

            var product = await productRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
            {
                return new ServicesResponse(false, "Product not found.");
            }

            var currentCount = product.ReviewsCount;
            var newCount = currentCount + 1;
            product.ReviewsCount = newCount;
            product.AverageRating = newCount == 0
                ? dto.Rating
                : (product.AverageRating * currentCount + dto.Rating) / newCount;

            await productRepository.UpdateAsync(product);
            await reviewRepository.SaveChangesAsync();

            return new ServicesResponse(true, "Review added successfully.");
        }

        public async Task<ServicesResponse> HideReviewAsync(string reviewId)
        {
            var updated = await reviewRepository.UpdateReviewApprovalStatusAsync(reviewId, false);
            return updated
                ? new ServicesResponse(true, "Review hidden successfully.")
                : new ServicesResponse(false, "Review not found.");
        }

        public async Task<ServicesResponse> ShowReviewAsync(string reviewId)
        {
            var updated = await reviewRepository.UpdateReviewApprovalStatusAsync(reviewId, true);
            return updated
                ? new ServicesResponse(true, "Review shown successfully.")
                : new ServicesResponse(false, "Review not found.");
        }
    }
}
