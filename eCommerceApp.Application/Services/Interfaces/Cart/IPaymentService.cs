using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Application.Services.Interfaces.Cart
{
    public interface IPaymentService
    {
        Task<ServicesResponse> Pay(decimal amount,
            IEnumerable<Product> cartProducts,
            IEnumerable<ProcessCart> carts);

    }
}
