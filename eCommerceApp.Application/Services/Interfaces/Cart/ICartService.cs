using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Application.Services.Interfaces.Cart
{
    public interface ICartService
    {
        Task<ServicesResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves);


        Task<ServicesResponse> Checkout(Checkout checkout);
        Task<IEnumerable<GetAchieve>> GetAchieves();
    }
    
}
