using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Application.Services.Interfaces.Cart
{
    public interface ICartService
    {
        Task<ServicesResponse> AddToCart(string userId, int productId, int quantity);
        Task<IEnumerable<GetCartDto>> GetMyCart(string userId);
        Task<ServicesResponse> RemoveFromCart(string userId, int productId);

        Task<ServicesResponse> Checkout(string userId, int paymentMethodId);

        Task<ServicesResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves);
        Task<IEnumerable<GetAchieve>> GetAchieves();
    }

}
