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

            var paymentResult = await paymentGateway.CreatePaymentIntentAsync(paymentRequest, userId);

            if (paymentResult.IsSuccess && paymentResult.Data != null)
            {
                order.PaymentIntentId = paymentResult.Data.PaymentIntentId;
                order.PaymentStatus = paymentResult.Data.Status;
                await orderRepository.UpdateAsync(order);
                await orderRepository.SaveChangesAsync();
            }

            return paymentResult;
        }

        public async Task<PaymentResult<CreatePaymentIntentResponseDto>> PayOrderAsync(PayOrderRequestDto request, string userId)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Order not found.");
            }

            if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.PaymentFailed)
            {
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure($"Order is not payable. Current status: {order.Status}");
            }

            if (order.TotalAmount <= 0)
            {
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Order total must be greater than zero.");
            }

            var paymentRequest = new CreatePaymentIntentRequestDto
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Currency = request.Currency ?? "usd",
                Description = request.Description ?? $"Order {order.Id}"
            };

            return await CreatePaymentIntentAsync(paymentRequest, userId);
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
