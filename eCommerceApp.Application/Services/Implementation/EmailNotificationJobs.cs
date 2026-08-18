using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Services.Implementation
{
    public class EmailNotificationJobs(
        IApplicationDbContext context,
        IEmailService emailService) : IEmailNotificationJobs
    {
        public async Task SendOrderConfirmationAsync(int orderId)
        {
            var order = await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.User == null || string.IsNullOrWhiteSpace(order.User.Email))
                return;

            var subject = $"Your Order #{order.Id} is Confirmed!";
            var body = $@"
                <h1>Thank you for your purchase!</h1>
                <p>Hi {order.User.FullName},</p>
                <p>We've received your payment and your order is now being processed.</p>
                <p><strong>Order ID:</strong> {order.Id}</p>
                <p><strong>Total Amount:</strong> {order.TotalAmount:C}</p>
                <p>We'll notify you again once your order has shipped.</p>
                <p>Thanks for shopping with us!</p>";

            await emailService.SendEmailAsync(order.User.Email, subject, body);
        }

        public async Task SendShippingConfirmationAsync(int orderId)
        {
            var order = await context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.User == null || string.IsNullOrWhiteSpace(order.User.Email))
                return;

            if (order.Status != OrderStatus.Shipped)
                return;

            var subject = $"Your Order #{order.Id} Has Shipped!";
            var body = $@"
                <h1>Your order is on the way!</h1>
                <p>Hi {order.User.FullName},</p>
                <p>Your order #{order.Id} has been shipped and is now in transit.</p>
                <p><strong>Total Amount:</strong> {order.TotalAmount:C}</p>
                <p>We will send you another update when the delivery is complete.</p>
                <p>Thanks for shopping with us!</p>";

            await emailService.SendEmailAsync(order.User.Email, subject, body);
        }

        public async Task SendDeliveryReminderAsync(int orderId)
        {
            var order = await context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.User == null || string.IsNullOrWhiteSpace(order.User.Email))
                return;

            if (order.Status != OrderStatus.Shipped)
                return;

            var subject = $"Delivery Reminder for Order #{order.Id}";
            var body = $@"
                <h1>Delivery reminder</h1>
                <p>Hi {order.User.FullName},</p>
                <p>This is a friendly reminder that your order #{order.Id} is on the way and should be delivered soon.</p>
                <p>If you have any questions, please contact support.</p>
                <p>Thanks for shopping with us!</p>";

            await emailService.SendEmailAsync(order.User.Email, subject, body);
        }

        public async Task SendReviewReminderAsync(int orderId)
        {
            var order = await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.User == null || string.IsNullOrWhiteSpace(order.User.Email))
                return;

            if (order.Status != OrderStatus.Delivered)
                return;

            var productIds = order.OrderItems?.Select(oi => oi.ProductId).Distinct().ToList() ?? new List<int>();
            var hasReview = await context.ProductReviews
                .AnyAsync(r => r.UserId == order.UserId && productIds.Contains(r.ProductId));

            if (hasReview)
                return;

            var subject = $"How was your order #{order.Id}?";
            var body = $@"
                <h1>We would love your feedback</h1>
                <p>Hi {order.User.FullName},</p>
                <p>Your order #{order.Id} has been delivered.</p>
                <p>Please take a moment to leave a review for the products you purchased.</p>
                <p>Your feedback helps us improve and helps other buyers.</p>
                <p>Thanks for shopping with us!</p>";

            await emailService.SendEmailAsync(order.User.Email, subject, body);
        }

        public async Task SendCartReminderAsync(string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
                return;

            var cart = await context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                return;

            var subject = "Your cart is waiting";
            var body = $@"
                <h1>Your cart is waiting</h1>
                <p>Hi {user.FullName},</p>
                <p>You still have items in your cart and we would love to help you complete your order.</p>
                <p>Return to your cart to finish checkout before your items are gone.</p>
                <p>Thanks for shopping with us!</p>";

            await emailService.SendEmailAsync(user.Email, subject, body);
        }
    }
}
