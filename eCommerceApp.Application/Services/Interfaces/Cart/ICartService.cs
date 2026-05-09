using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Application.Services.Interfaces.Cart
{
    public interface ICartService
    {
        Task<ServicesResponse> AddToCart(string userId, int productId, int quantity);
        Task<IEnumerable<GetCartDto>> GetMyCart(string userId);
        Task<ServicesResponse> IncrementCartItemQuantity(int itemId);
        Task<ServicesResponse> RemoveCartItem(int itemId);

        Task<ServicesResponse> DecrementCartItemQuantity(int itemId);
        Task<ServicesResponse> Checkout(string userId, int paymentMethodId);

        Task<ServicesResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves);
        Task<IEnumerable<GetAchieve>> GetAchieves();
    }

}
