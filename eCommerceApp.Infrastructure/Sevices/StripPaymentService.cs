using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.CartEntities;
using Stripe.Checkout;

namespace eCommerceApp.Infrastructure.Sevices
{
   public class StripPaymentService : IPaymentService
{
    public async Task<ServicesResponse> Pay(
        decimal amount,
        IEnumerable<CartItem> cartItems,
        IEnumerable<Product> products)
    {
        try
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                if (product == null)
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
                SuccessUrl = "http://localhost:5214/payment/success",
                CancelUrl = "http://localhost:5214/payment/cancel",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return new ServicesResponse(true, session.Url);
        }
        catch (Exception ex)
        {
            return new ServicesResponse(false, ex.Message);
        }
    }
}
}
