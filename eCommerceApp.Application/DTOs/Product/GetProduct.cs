using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Product
{
    public class GetProduct :Productbase
    {
        [Required]
        public int Id { get; set; }
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
