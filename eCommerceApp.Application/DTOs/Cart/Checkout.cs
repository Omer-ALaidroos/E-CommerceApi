using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class Checkout
    {

        [Required]
        public required int PaymentMethodId { get; set; }

        [Required] public int UserId { get; set; }

        [Required]
        public required int ShippingAddressId { get; set; }

        [Required]
        public required IEnumerable<ProcessCart> Carts;
    }
}
