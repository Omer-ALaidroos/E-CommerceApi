using eCommerceApp.Application.DTOs.Payment;

namespace eCommerceApp.Application.Services.Interfaces.Payment
{
    public interface IPaymentRepository
    {
        Task<PaymentStatusResponseDto?> GetPaymentStatusByOrderIdAsync(int orderId, string userId);
        Task<bool> SavePaymentStatusAsync(PaymentStatusResponseDto paymentStatus);
    }
}
