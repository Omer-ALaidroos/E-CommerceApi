using eCommerceApp.Application.Contracts.Payment;
using eCommerceApp.Application.DTOs.Payment;

namespace eCommerceApp.Application.Services.Interfaces.Payment
{
    public interface IPaymentGateway
    {
        Task<PaymentResult<CreatePaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentRequestDto request, string userId);
        Task<PaymentResult<PaymentStatusResponseDto>> ConfirmPaymentAsync(ConfirmPaymentRequestDto request, string userId);
        Task<PaymentResult<PaymentStatusResponseDto>> GetPaymentStatusAsync(int orderId, string userId);
    }
}
