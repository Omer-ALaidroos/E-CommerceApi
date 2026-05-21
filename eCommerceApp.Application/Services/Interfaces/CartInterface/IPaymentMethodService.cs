using eCommerceApp.Application.DTOs.Cart;

namespace eCommerceApp.Application.Services.Interfaces.CartInterface
{
    public interface IPaymentMethodService
    {
        Task <IEnumerable< GetPaymntMethod>> GetPaymntMethods();
    }
}
