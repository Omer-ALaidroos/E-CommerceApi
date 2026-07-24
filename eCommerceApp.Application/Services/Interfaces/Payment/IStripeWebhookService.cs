using eCommerceApp.Application.Contracts.Payment;
using eCommerceApp.Application.DTOs.Payment;

namespace eCommerceApp.Application.Services.Interfaces.Payment
{
    public interface IStripeWebhookService
    {
        Task<PaymentResult<PaymentStatusResponseDto>> HandleWebhookAsync(string payload, string stripeSignature);
    }
}
