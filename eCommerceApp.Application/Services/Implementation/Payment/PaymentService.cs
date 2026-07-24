using eCommerceApp.Application.Contracts.Payment;
using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Application.Services.Interfaces.Payment;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementation.Payment
{
    public class PaymentService(
        IPaymentGateway paymentGateway,
        IAppLogger<PaymentService> logger,
        IOrder orderRepository) : IPaymentService
    {
        public async Task<PaymentResult<CreatePaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentRequestDto request, string userId)
        {
            logger.LogInformation($"Creating Stripe payment intent for order {request.OrderId}.");

            var order = await orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Order not found.");
            }

            if (order.TotalAmount <= 0)
            {
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Order total must be greater than zero.");
            }

            var paymentRequest = new CreatePaymentIntentRequestDto
            {
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                Currency = "usd",
                Description = request.Description ?? $"Order {request.OrderId}"
            };

            return await paymentGateway.CreatePaymentIntentAsync(paymentRequest, userId);
        }

        public async Task<PaymentResult<PaymentStatusResponseDto>> ConfirmPaymentAsync(ConfirmPaymentRequestDto request, string userId)
        {
            logger.LogInformation($"Confirming Stripe payment intent {request.PaymentIntentId}.");
            return await paymentGateway.ConfirmPaymentAsync(request, userId);
        }

        public async Task<PaymentResult<PaymentStatusResponseDto>> GetPaymentStatusAsync(int orderId, string userId)
        {
            logger.LogInformation($"Retrieving Stripe payment status for order {orderId}.");
            return await paymentGateway.GetPaymentStatusAsync(orderId, userId);
        }
    }
}
