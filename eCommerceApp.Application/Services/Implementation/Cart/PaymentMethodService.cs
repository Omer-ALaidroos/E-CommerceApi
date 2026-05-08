using AutoMapper;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Interfaces.CartInterface;
namespace eCommerceApp.Application.Services.Implementation.Cart
{
    public class PaymentMethodService(IPaymentMethod paymentMethod, IMapper mapper) : IPaymentMethodService
    {
        public async Task<IEnumerable<GetPaymntMethod>> GetPaymntMethods()
        {
            var paymentMethods = await paymentMethod.GetPaymentMethods();

            if (!paymentMethods.Any()) return [];

            return mapper.Map<IEnumerable<GetPaymntMethod>>(paymentMethods);

        }
    }
}
