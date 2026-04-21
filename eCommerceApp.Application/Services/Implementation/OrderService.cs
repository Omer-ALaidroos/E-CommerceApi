using AutoMapper;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Entities;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Orders;
using System.Net;

namespace eCommerceApp.Application.Services.Implementation
{
    public class OrderService(IGeneric<Order> orderInterface, IMapper mapper) : IOrderService
    {
        public async Task<ServicesResponse> CreateOrder(Checkout checkout,decimal totalAmount)
        {
            CreateOrder createOrder = new CreateOrder
            {
                UserId = checkout.UserId,
                PaymentMethodId = checkout.PaymentMethodId,
                ShippingAddressId = checkout.ShippingAddressId,
                TotalAmount =totalAmount
            };

            Order mappedOrder = mapper.Map<Order>(createOrder);
            int OrderId = await orderInterface.AddAsync(mappedOrder);

            if (OrderId > 0)
            {

                return new ServicesResponse(true, "Order added successfully.");

            }
            else
            {
                return new ServicesResponse(false, "Failed to add Order.");

            }
        }

        public Task<ServicesResponse> CancelORderAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async  Task<ServicesResponse> DeleteORderAsync(int id)
        {
            int result = await orderInterface.DeleteAsync(id);




            return result > 0 ?
                new ServicesResponse(true, "Order delete successfully.") :
                new ServicesResponse(false, "Failed to delete Order."); ;
        }

        public async Task<IEnumerable<GetOrder>> GetAllAsync()
        {
            var Addresses = await orderInterface.GetAllAsync();

            if (!Addresses.Any()) return [];

            return mapper.Map<IEnumerable<GetOrder>>(Addresses);
        }

        public async Task<GetOrder> GetByIdAsync(int id)
        {
            var Address = await orderInterface.GetByIdAsync(id);

            if (Address == null) return null;

            return mapper.Map<GetOrder>(Address);
        }

        public Task<ServicesResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto orderstatus)
        {
            throw new NotImplementedException();
        }
    }
}
