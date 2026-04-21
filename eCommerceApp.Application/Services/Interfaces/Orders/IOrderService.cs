using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Domain.Interfaces.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<GetOrder>> GetAllAsync();

        Task<GetOrder> GetByIdAsync(int id);
       
        Task<ServicesResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto orderstatus);
        Task<ServicesResponse> CancelORderAsync(int id);
        Task<ServicesResponse> DeleteORderAsync(int id);

    }
}
