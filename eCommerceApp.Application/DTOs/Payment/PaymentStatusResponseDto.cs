namespace eCommerceApp.Application.DTOs.Payment
{
    public class PaymentStatusResponseDto
    {
        public int OrderId { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
