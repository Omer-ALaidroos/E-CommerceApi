namespace eCommerceApp.Application.DTOs.Product
{
    public class CanReviewResponseDto
    {
        public bool CanReview { get; set; }
        public bool HasPurchased { get; set; }
        public bool AlreadyReviewed { get; set; }
    }
}