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

        Task<ServicesResponse> UpdateOrderStatusAsync(int orderId);
        Task<ServicesResponse> CancelOrderAsync(int id);
        Task<ServicesResponse> DeleteOrderAsync(int id);

    }
}
