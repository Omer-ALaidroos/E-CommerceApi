using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<GetOrder>> GetAllAsync();
        Task<ServicesResponse> CreateOrder(Checkout checkout);
      //Task<IEnumerable<Product?> GetByUserIdAsync(string userId)
       
        Task<ServicesResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto orderstatus);
        Task<ServicesResponse> CancelORderAsync(int id);
        Task<ServicesResponse> DeleteORderAsync(int id);

    }
}
