using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class Checkout
    {

        [Required]
        public required int PaymentMethodId { get; set; }

       public string? UserId { get; set; }

       
        public  int ShippingAddressId { get; set; }

        
    }
}
