using eCommerceApp.Application.DTOs.Payment;

namespace eCommerceApp.Application.Services.Interfaces.Payment
{
    public interface IPaymentEventHandler
    {
        Task HandlePaymentSucceededAsync(PaymentStatusResponseDto paymentStatus);
        Task HandlePaymentFailedAsync(PaymentStatusResponseDto paymentStatus);
    }
}
