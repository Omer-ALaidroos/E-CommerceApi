using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Payment
{
    public class ConfirmPaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;

        [Required]
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}
