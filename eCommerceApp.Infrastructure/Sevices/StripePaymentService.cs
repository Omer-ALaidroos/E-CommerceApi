using eCommerceApp.Application.Contracts.Payment;
using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Application.Services.Interfaces.Payment;
using eCommerceApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Stripe;

namespace eCommerceApp.Infrastructure.Sevices
{
    public class StripePaymentService : IPaymentGateway
    {
        private readonly StripeSettings _settings;
        private readonly IAppLogger<StripePaymentService> _logger;

        public StripePaymentService(IOptions<StripeSettings> options, IAppLogger<StripePaymentService> logger)
        {
            _settings = options.Value;
            _logger = logger;

            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<PaymentResult<CreatePaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentIntentRequestDto request, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.SecretKey))
                {
                    return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Stripe secret key is not configured.");
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100),
                    Currency = request.Currency.ToLowerInvariant(),
                    PaymentMethodTypes = new List<string> { "card" },
                    Description = request.Description ?? $"Order {request.OrderId}",
                    Metadata = new Dictionary<string, string>
                    {
                        ["order_id"] = request.OrderId.ToString(),
                        ["user_id"] = userId
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                var response = new CreatePaymentIntentResponseDto
                {
                    OrderId = request.OrderId,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = request.Amount,
                    Currency = request.Currency.ToUpperInvariant()
                };

                return PaymentResult<CreatePaymentIntentResponseDto>.Success(response, "Payment intent created successfully.");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe payment intent creation failed.");
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("Stripe payment intent creation failed.", ex.StripeError?.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating Stripe payment intent.");
                return PaymentResult<CreatePaymentIntentResponseDto>.Failure("An unexpected error occurred while creating the payment intent.");
            }
        }

        public async Task<PaymentResult<PaymentStatusResponseDto>> ConfirmPaymentAsync(ConfirmPaymentRequestDto request, string userId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                var response = new PaymentStatusResponseDto
                {
                    OrderId = request.OrderId,
                    PaymentIntentId = paymentIntent.Id,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount / 100m,
                    Currency = paymentIntent.Currency.ToUpperInvariant(),
                    Message = paymentIntent.Status switch
                    {
                        "succeeded" => "Payment succeeded.",
                        "requires_payment_method" => "Payment requires a new payment method.",
                        "requires_confirmation" => "Payment requires confirmation.",
                        _ => "Payment status retrieved."
                    }
                };

                return PaymentResult<PaymentStatusResponseDto>.Success(response, "Payment status retrieved successfully.");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe payment confirmation failed.");
                return PaymentResult<PaymentStatusResponseDto>.Failure("Stripe payment confirmation failed.", ex.StripeError?.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while confirming Stripe payment.");
                return PaymentResult<PaymentStatusResponseDto>.Failure("An unexpected error occurred while confirming the payment.");
            }
        }

        public async Task<PaymentResult<PaymentStatusResponseDto>> GetPaymentStatusAsync(int orderId, string userId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntentId = string.Empty;

                var response = new PaymentStatusResponseDto
                {
                    OrderId = orderId,
                    PaymentIntentId = paymentIntentId,
                    Status = "unknown",
                    Amount = 0,
                    Currency = "USD",
                    Message = "Payment status is not available yet."
                };

                return PaymentResult<PaymentStatusResponseDto>.Success(response, "Payment status retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving Stripe payment status.");
                return PaymentResult<PaymentStatusResponseDto>.Failure("An unexpected error occurred while retrieving the payment status.");
            }
        }
    }
}
