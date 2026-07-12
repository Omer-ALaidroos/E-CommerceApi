using eCommerceApp.Application.DTOs.Category;
using System.Collections.Generic;

namespace eCommerceApp.Application.DTOs.Product
{
    public class GetProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }
        public ICollection<ProductReviewDto> Reviews { get; set; }
    }
}
