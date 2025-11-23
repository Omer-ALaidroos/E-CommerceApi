using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class CreateAchieve
    { 
        [Required]
        public int productId { get; set; }
        [Required]
        public int quantity { get; set; }
        [Required]
        public string? UserId { get; set; }
    }
}
