namespace eCommerceApp.Application.DTOs.Product
{
    public class ProductReviewDto
    {
       public string Id { get; set; }
        public ProductReviewUserDto User { get; set; } = null!;
        public int Rating { get; set; }
        public string Review { get; set; }

        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
