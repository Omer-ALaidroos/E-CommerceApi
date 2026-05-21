using AutoMapper;
using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.CartInterface;
using eCommerceApp.Domain.Entities.Orders;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Orders;
using ECommerce.Core.Entities;

namespace eCommerceApp.Application.Services.Implementation
{
    public class OrderService(
        IOrder orderInterface,
        IProduct productRepository,
        IMapper mapper,
        ICartService cartService
    ) : IOrderService
    {

        public async Task<ServicesResponse> CreateOrder(Checkout checkout)
        {
            var cart = await cartService.GetActiveCart(checkout.UserId);

            if (cart == null || !cart.Items.Any())
            {
                return new ServicesResponse(false, "Cart is empty.");
            }

            var productIds = cart.Items
                .Select(c => c.ProductId)
                .ToList();

            var products = await productRepository.GetByIdsAsync(productIds);

            var orderItems = new List<OrderItem>();

            decimal totalAmount = 0;

            // Validate stock and prepare order items
            foreach (var item in cart.Items)
            {
                var product = products
                    .FirstOrDefault(p => p.Id == item.ProductId);

                if (product == null)
                {
                    return new ServicesResponse(
                        false,
                        $"Product with ID {item.ProductId} not found."
                    );
                }

                if (product.Quantity < item.Quantity)
                {
                    return new ServicesResponse(
                        false,
                        $"Insufficient stock for product {product.Name}. " +
                        $"Available: {product.Quantity}, Requested: {item.Quantity}."
                    );
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                orderItems.Add(orderItem);

                totalAmount += item.Quantity * product.Price;
            }

            using var transaction =
                await orderInterface.BeginTransactionAsync();

            try
            {
                // Decrease product stock
                foreach (var item in cart.Items)
                {
                   
                        await productRepository
                            .DecreaseProductQuantityAsync(
                                item.ProductId,
                                item.Quantity
                            );
                   int decreased = await productRepository.SaveChangesAsync();

                    if (decreased <= 0)
                    {
                        await transaction.RollbackAsync();

                        return new ServicesResponse(
                            false,
                            "Failed to update inventory."
                        );
                    }
                }

                // Create order
                var order = new Order
                {
                    UserId = checkout.UserId,
                    PaymentMethodId = checkout.PaymentMethodId,
                    ShippingAddressId = checkout.ShippingAddressId,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    OrderItems = orderItems
                };

                await orderInterface.AddAsync(order);

                // Save order
                int result = await orderInterface.SaveChangesAsync();

                if (result <= 0)
                {
                    await transaction.RollbackAsync();

                    return new ServicesResponse(
                        false,
                        "Failed to create order."
                    );
                }

                // Clear cart
                await cartService.ClearCartAsync(checkout.UserId);

                // Commit transaction
                await transaction.CommitAsync();

                return new ServicesResponse(
                    true,
                    "Order created successfully."
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServicesResponse> CancelOrderAsync(int id)
        {
            var order = await orderInterface.GetByIdAsync(id);

            if (order == null)
            {
                return new ServicesResponse(
                    false,
                    $"Order with ID {id} not found."
                );
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                return new ServicesResponse(
                    false,
                    "Order already cancelled."
                );
            }

            if (order.Status == OrderStatus.Shipped)
            {
                return new ServicesResponse(
                    false,
                    "Shipped orders cannot be cancelled."
                );
            }

            using var transaction =
                await orderInterface.BeginTransactionAsync();

            try
            {
                // Restore stock
                foreach (var item in order.OrderItems)
                {
                    await productRepository
                        .IncreaseProductQuantityAsync(
                            item.ProductId,
                            item.Quantity
                        );
                }

                // Update order status
                order.Status = OrderStatus.Cancelled;

                await orderInterface.UpdateAsync(order);

                int result = await orderInterface.SaveChangesAsync();

                if (result <= 0)
                {
                    await transaction.RollbackAsync();

                    return new ServicesResponse(
                        false,
                        "Failed to cancel order."
                    );
                }

                await transaction.CommitAsync();

                return new ServicesResponse(
                    true,
                    "Order cancelled successfully."
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServicesResponse> DeleteOrderAsync(int id)
        {
            using var transaction =
                await orderInterface.BeginTransactionAsync();

            try
            {
                await orderInterface.DeleteAsync(id);

                int result = await orderInterface.SaveChangesAsync();

                if (result <= 0)
                {
                    await transaction.RollbackAsync();

                    return new ServicesResponse(
                        false,
                        "Failed to delete order."
                    );
                }

                await transaction.CommitAsync();

                return new ServicesResponse(
                    true,
                    "Order deleted successfully."
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<GetOrder>> GetAllAsync()
        {
            var orders = await orderInterface.GetAllAsync();

            if (!orders.Any())
            {
                return [];
            }

            return mapper.Map<IEnumerable<GetOrder>>(orders);
        }

        public async Task<GetOrder> GetByIdAsync(int id)
        {
            var order = await orderInterface.GetByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            return mapper.Map<GetOrder>(order);
        }

        public async Task<ServicesResponse> UpdateOrderStatusAsync(
            UpdateOrderStatusDto orderstatus
        )
        {
            var order =
                await orderInterface.GetByIdAsync(orderstatus.Id);

            if (order == null)
            {
                return new ServicesResponse(
                    false,
                    "Order not found."
                );
            }

            if (!Enum.TryParse<OrderStatus>(orderstatus.Status, true, out var status))
            {
                return new ServicesResponse(false, "Invalid status value.");
            }

            order.Status = status;

            await orderInterface.UpdateAsync(order);

            int result = await orderInterface.SaveChangesAsync();

            return result > 0
                ? new ServicesResponse(
                    true,
                    "Order status updated successfully."
                )
                : new ServicesResponse(
                    false,
                    "Failed to update order status."
                );
        }

        public Task<ServicesResponse> CancelORderAsync(int id)
        {
            return CancelOrderAsync(id);
        }

        public Task<ServicesResponse> DeleteORderAsync(int id)
        {
            return DeleteOrderAsync(id);
        }
    }
}