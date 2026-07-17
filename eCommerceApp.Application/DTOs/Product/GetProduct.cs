using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Product
{
    public class GetProduct :Productbase
    {
        [Required]
        public int Id { get; set; }
        public string? PrimaryImageUrl { get; set; }

        public bool IsFavorite { get; set; }

        public double AverageRating { get; set; }

        public int CategoryId { get; set; }
       
    }
}
