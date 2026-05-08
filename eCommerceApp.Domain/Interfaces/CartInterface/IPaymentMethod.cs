using eCommerceApp.Domain.Entities.CartEntities;

namespace eCommerceApp.Domain.Interfaces.CartInterface
{
    public interface IPaymentMethod
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
    }
}
