using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Payment
{
    public class PayOrderRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [StringLength(3)]
        public string? Currency { get; set; } = "usd";

        public string? Description { get; set; }
    }
}
