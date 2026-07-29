using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Contracts.Payment;

namespace eCommerceApp.Application.Services.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<PaymentResult<CreatePaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentRequestDto request, string userId);
        Task<PaymentResult<CreatePaymentIntentResponseDto>> PayOrderAsync(PayOrderRequestDto request, string userId);
        Task<PaymentResult<PaymentStatusResponseDto>> ConfirmPaymentAsync(ConfirmPaymentRequestDto request, string userId);
        Task<PaymentResult<PaymentStatusResponseDto>> GetPaymentStatusAsync(int orderId, string userId);
    }
}
