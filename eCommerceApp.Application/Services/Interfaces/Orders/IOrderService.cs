using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Entities.Orders;

namespace eCommerceApp.Domain.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<GetOrder>> GetAllAsync();
        Task<ServicesResponse> CreateOrder(Checkout checkout);

        Task<ServicesResponse> UpdateOrderStatusAsync(int orderId);
        Task<ServicesResponse> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<ServicesResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto orderStatusDto);
        Task<ServicesResponse> GetOrderStatusByIdAsync(int orderId);
        Task<ServicesResponse> CancelOrderAsync(int id);
        Task<ServicesResponse> DeleteOrderAsync(int id);
    }
}
