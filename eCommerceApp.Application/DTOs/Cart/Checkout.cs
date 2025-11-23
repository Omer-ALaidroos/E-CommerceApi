using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class Checkout
    {

        [Required]
        public required int PaymentMethodId { get; set; }
        [Required]
        public required IEnumerable<ProcessCart> Carts;
    }
}
