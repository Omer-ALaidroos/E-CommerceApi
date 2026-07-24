using eCommerceApp.Application.Contracts.Payment;
using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Application.Services.Interfaces.Payment;
using eCommerceApp.Domain.Interfaces.Orders;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace eCommerceApp.Application.Services.Implementation.Payment
{
    public class StripeWebhookService(
        IConfiguration configuration,
        IAppLogger<StripeWebhookService> logger,
        IOrderService orderService) : IStripeWebhookService
    {
        public async Task<PaymentResult<PaymentStatusResponseDto>> HandleWebhookAsync(
    string payload,
    string stripeSignature)
        {

            try
            {

                var webhookSecret = configuration["Stripe:WebhookSecret"];

                if (string.IsNullOrWhiteSpace(webhookSecret))
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "Stripe webhook secret is not configured.");
                }

                if (string.IsNullOrWhiteSpace(stripeSignature))
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "Stripe signature is missing.");
                }

                if (string.IsNullOrWhiteSpace(payload))
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "Webhook payload is empty.");
                }

                // Verify webhook signature
                var stripeEvent = EventUtility.ConstructEvent(
    payload,
    stripeSignature,
    webhookSecret,
    throwOnApiVersionMismatch: false);
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                if (paymentIntent == null)
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "PaymentIntent not found in webhook.");
                }

                if (!paymentIntent.Metadata.TryGetValue("order_id", out var orderIdValue))
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "Order id was not found in metadata.");
                }

                if (!int.TryParse(orderIdValue, out var orderId))
                {
                    return PaymentResult<PaymentStatusResponseDto>.Failure(
                        "Invalid order id.");
                }

                var response = new PaymentStatusResponseDto
                {
                    OrderId = orderId,
                    PaymentIntentId = paymentIntent.Id,
                    Status = stripeEvent.Type,
                    Amount = paymentIntent.Amount / 100m,
                    Currency = paymentIntent.Currency?.ToUpperInvariant() ?? "USD",
                    Message = stripeEvent.Type
                };

               
// Replace all instances of Events.PaymentIntentSucceeded, Events.PaymentIntentPaymentFailed, and Events.PaymentIntentCanceled
// with the correct Stripe event type string constants.

         
                        switch (stripeEvent.Type)
                        {
                            case "payment_intent.succeeded":
                                {
                                    var result = await orderService.UpdateOrderStatusAsync(
                                        orderId,
                                        OrderStatus.Paid);

                                    if (!result.IsSuccess)
                                        return PaymentResult<PaymentStatusResponseDto>.Failure(result.Message);

                                    response.Message = "Payment succeeded.";

                                    break;
                                }

                            case "payment_intent.payment_failed":
                                {
                                    var result = await orderService.UpdateOrderStatusAsync(
                                        orderId,
                                        OrderStatus.PaymentFailed);

                                    if (!result.IsSuccess)
                                        return PaymentResult<PaymentStatusResponseDto>.Failure(result.Message);

                                    response.Message = "Payment failed.";

                                    break;
                                }

                            case "payment_intent.canceled":
                                {
                                    var result = await orderService.UpdateOrderStatusAsync(
                                        orderId,
                                        OrderStatus.PaymentFailed);

                                    if (!result.IsSuccess)
                                        return PaymentResult<PaymentStatusResponseDto>.Failure(result.Message);

                                    response.Message = "Payment canceled.";

                                    break;
                                }

                            default:
                                response.Message = $"Unhandled event: {stripeEvent.Type}";
                                break;
                        }
                  

                return PaymentResult<PaymentStatusResponseDto>.Success(
                    response,
                    "Webhook processed successfully.");
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe webhook signature verification failed.");

                return PaymentResult<PaymentStatusResponseDto>.Failure(
                    $"Stripe signature verification failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stripe webhook processing failed.");

                return PaymentResult<PaymentStatusResponseDto>.Failure(
                    "Unexpected error while processing webhook.");
            }
        }

    }
}
