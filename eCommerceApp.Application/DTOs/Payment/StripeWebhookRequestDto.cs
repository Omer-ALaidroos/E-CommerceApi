namespace eCommerceApp.Application.DTOs.Payment
{
    public class StripeWebhookRequestDto
    {
        public string Payload { get; set; } = string.Empty;
        public string StripeSignature { get; set; } = string.Empty;
    }
}
