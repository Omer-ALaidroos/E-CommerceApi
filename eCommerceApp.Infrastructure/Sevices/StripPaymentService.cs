using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.Services.Interfaces.CartInterface;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.CartEntities;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace eCommerceApp.Infrastructure.Sevices
{
   public class StripPaymentService(IConfiguration configuration, IAppLogger<StripPaymentService> logger) : IPaymentService
{
    public async Task<ServicesResponse> Pay(
        decimal amount,
        IEnumerable<CartItem> cartItems,
        IEnumerable<Product> products)
    {
        try
        {
            var productDict = products.ToDictionary(p => p.Id);
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartItems)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    continue;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name,
                            Description = product.Description
                        },

                        UnitAmountDecimal = (long)(item.PriceAtTime * 100),
                    },
                    Quantity = item.Quantity,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = configuration["Stripe:SuccessUrl"],
                CancelUrl = configuration["Stripe:CancelUrl"],
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return new ServicesResponse(true, session.Url);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during Stripe session creation.");
            return new ServicesResponse(false, "An error occurred while processing your payment.");
        }
    }
}
}
