using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class Checkout
    {

        [Required]
        public required int PaymentMethodId { get; set; }

       public string? UserId { get; set; }

        [Required]
        public required int ShippingAddressId { get; set; }

        
    }
}
