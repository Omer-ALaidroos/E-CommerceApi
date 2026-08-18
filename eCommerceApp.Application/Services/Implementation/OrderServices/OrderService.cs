using AutoMapper;
using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Application.Services.Interfaces.CartInterface;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Orders;
using ECommerce.Core.Entities;
using eCommerceApp.Domain.Entities.Orders;
using Hangfire;

namespace eCommerceApp.Application.Services.Implementation.OrderServices
{
    public class OrderService(
        IOrder orderInterface,
        IProduct productRepository,
        IMapper mapper,
        ICartService cartService,
        IBackgroundJobClient backgroundJobs,
        IEmailNotificationJobs emailNotificationJobs
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
                // Create order
                var order = new Order
                {
                    UserId = checkout.UserId,
                    PaymentMethodId = checkout.PaymentMethodId,
                    ShippingAddressId = checkout.ShippingAddressId,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.PendingPayment,
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
                    "Order created successfully.",
                    order.Id
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServicesResponse> ProcessSuccessfulPaymentAsync(int orderId, string paymentIntentId)
        {
            using var transaction = await orderInterface.BeginTransactionAsync();

            try
            {
                var order = await orderInterface.GetByIdAsync(orderId);

                if (order == null)
                {
                    return new ServicesResponse(false, "Order not found.");
                }

                if (order.Status == OrderStatus.Paid && string.Equals(order.PaymentStatus, "succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    return new ServicesResponse(true, "Payment already processed.", order.Id);
                }

                if (order.Status != OrderStatus.PendingPayment)
                {
                    return new ServicesResponse(false, $"Order is no longer pending payment. Current status: {order.Status}.");
                }

                var orderItems = order.OrderItems?.ToList() ?? [];

                foreach (var item in orderItems)
                {
                    var product = await productRepository.GetByIdAsync(item.ProductId);

                    if (product == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServicesResponse(false, $"Product with ID {item.ProductId} was not found.");
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return new ServicesResponse(false, $"Insufficient stock for product {product.Name}.");
                    }

                    product.Quantity -= item.Quantity;
                    await productRepository.UpdateAsync(product);
                }

                int inventoryResult = await productRepository.SaveChangesAsync();

                if (inventoryResult <= 0)
                {
                    await transaction.RollbackAsync();
                    return new ServicesResponse(false, "Failed to update inventory.");
                }

                order.PaymentIntentId = paymentIntentId;
                order.PaymentStatus = "succeeded";
                order.Status = OrderStatus.Paid;

                await orderInterface.UpdateAsync(order);

                int result = await orderInterface.SaveChangesAsync();

                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    return new ServicesResponse(false, "Failed to update order payment state.");
                }

                await transaction.CommitAsync();

                backgroundJobs.Enqueue(() => emailNotificationJobs.SendOrderConfirmationAsync(order.Id));

                return new ServicesResponse(true, "Payment succeeded and stock updated.", order.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServicesResponse> ProcessFailedPaymentAsync(int orderId, string paymentIntentId)
        {
            using var transaction = await orderInterface.BeginTransactionAsync();

            try
            {
                var order = await orderInterface.GetByIdAsync(orderId);

                if (order == null)
                {
                    return new ServicesResponse(false, "Order not found.");
                }

                if (order.Status == OrderStatus.Paid)
                {
                    return new ServicesResponse(true, "Order already paid.", order.Id);
                }

                if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.PaymentFailed)
                {
                    return new ServicesResponse(false, $"Order status cannot be updated to payment failed from {order.Status}.");
                }

                order.PaymentIntentId = paymentIntentId;
                order.PaymentStatus = "failed";

                if (order.Status == OrderStatus.PendingPayment)
                {
                    order.Status = OrderStatus.PaymentFailed;
                }

                await orderInterface.UpdateAsync(order);

                int result = await orderInterface.SaveChangesAsync();

                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    return new ServicesResponse(false, "Failed to update order payment state.");
                }

                await transaction.CommitAsync();

                return new ServicesResponse(true, "Payment failed.", order.Id);
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

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            {
                return new ServicesResponse(
                    false,
                    "Shipped or delivered orders cannot be cancelled."
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
        public Task<ServicesResponse> UpdateOrderStatusAsync(int id)
        {
            return UpdateOrderStatusAsync(id, null);
        }

        public async Task<ServicesResponse> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await orderInterface.GetByIdAsync(id);

            if (order == null)
            {
                return new ServicesResponse(false, "Order not found.");
            }

            if (!IsValidTransition(order.Status, status))
            {
                return new ServicesResponse(
                    false,
                    $"Order status cannot transition from {order.Status} to {status}."
                );
            }

            order.Status = status;
            await orderInterface.UpdateAsync(order);

            int result = await orderInterface.SaveChangesAsync();

            return result > 0
                ? new ServicesResponse(true, "Order status updated successfully.")
                : new ServicesResponse(false, "Failed to update order status.");
        }

        public async Task<ServicesResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto orderStatusDto)
        {
            var order = await orderInterface.GetByIdAsync(orderStatusDto.Id);

            if (order == null)
            {
                return new ServicesResponse(false, "Order not found.");
            }

            if (!Enum.TryParse<OrderStatus>(orderStatusDto.Status, true, out var status))
            {
                return new ServicesResponse(false, "Invalid status value.");
            }

            if (!IsValidTransition(order.Status, status))
            {
                return new ServicesResponse(
                    false,
                    $"Order status cannot transition from {order.Status} to {status}."
                );
            }

            order.Status = status;
            await orderInterface.UpdateAsync(order);

            int result = await orderInterface.SaveChangesAsync();

            if (result > 0)
            {
                if (status == OrderStatus.Shipped)
                {
                    backgroundJobs.Enqueue(() => emailNotificationJobs.SendShippingConfirmationAsync(order.Id));
                    backgroundJobs.Schedule(() => emailNotificationJobs.SendDeliveryReminderAsync(order.Id), TimeSpan.FromDays(3));
                }

                if (status == OrderStatus.Delivered)
                {
                    backgroundJobs.Schedule(() => emailNotificationJobs.SendReviewReminderAsync(order.Id), TimeSpan.FromDays(2));
                }

                return new ServicesResponse(true, "Order status updated successfully.");
            }

            return new ServicesResponse(false, "Failed to update order status.");
        }

        public async Task<ServicesResponse> GetOrderStatusByIdAsync(int orderId)
        {
            var order = await orderInterface.GetByIdAsync(orderId);

            if (order == null)
            {
                return new ServicesResponse(false, "Order not found.");
            }

            return new ServicesResponse(
                true,
                "Order status retrieved successfully.",
                new { OrderId = order.Id, Status = order.Status.ToString() }
            );
        }

        private async Task<ServicesResponse> UpdateOrderStatusAsync(int id, OrderStatus? status)
        {
            var order = await orderInterface.GetByIdAsync(id);

            if (order == null)
            {
                return new ServicesResponse(false, "Order not found.");
            }

            if (status is null)
            {
                if(order.Status == OrderStatus.PendingPayment)
                {
                    return new ServicesResponse(false, "Order status cannot be progressed further.");
                }
                OrderStatus? nextStatus = order.Status switch
                {
                   
                    OrderStatus.Paid => OrderStatus.Processing,
                    OrderStatus.Processing => OrderStatus.Shipped,
                    OrderStatus.Shipped => OrderStatus.Delivered,
                    _ => null
                };

                if (nextStatus is null)
                {
                    return new ServicesResponse(false, "Order status cannot be progressed further.");
                }

                status = nextStatus;
            }

            if (!IsValidTransition(order.Status, status.Value))
            {
                return new ServicesResponse(
                    false,
                    $"Order status cannot transition from {order.Status} to {status.Value}."
                );
            }

            order.Status = status.Value;
            await orderInterface.UpdateAsync(order);

            int result = await orderInterface.SaveChangesAsync();

            return result > 0
                ? new ServicesResponse(true, "Order status updated successfully.")
                : new ServicesResponse(false, "Failed to update order status.");
        }

        private static bool IsValidTransition(OrderStatus currentStatus, OrderStatus targetStatus)
        {
            return (currentStatus, targetStatus) switch
            {
                (OrderStatus.PendingPayment, OrderStatus.Paid) => true,
                (OrderStatus.PendingPayment, OrderStatus.PaymentFailed) => true,
                (OrderStatus.PendingPayment, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Processing) => true,
                (OrderStatus.Paid, OrderStatus.PaymentFailed) => true,
                (OrderStatus.Paid, OrderStatus.Cancelled) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Processing, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.PaymentFailed, OrderStatus.Paid) => true,
                (OrderStatus.PaymentFailed, OrderStatus.Cancelled) => true,
                _ => false
            };
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