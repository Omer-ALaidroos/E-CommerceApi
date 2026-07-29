namespace eCommerceApp.Application.DTOs.Product
{
    public class CreateProductReviewDto
    {
       
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Review { get; set; }
    }
}
