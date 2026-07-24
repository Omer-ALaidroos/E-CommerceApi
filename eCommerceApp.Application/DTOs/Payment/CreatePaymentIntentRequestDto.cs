using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Payment
{
    public class CreatePaymentIntentRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "usd";

        public string? Description { get; set; }
    }
}
