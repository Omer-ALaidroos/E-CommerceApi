using eCommerceApp.Application.DTOs;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.CartEntities;

namespace eCommerceApp.Application.Services.Interfaces.CartInterface
{
    public interface IPaymentService
    {
        Task<ServicesResponse> Pay(
            decimal amount,
            IEnumerable<CartItem> cartItems,
            IEnumerable<Product> products);
    }
}
