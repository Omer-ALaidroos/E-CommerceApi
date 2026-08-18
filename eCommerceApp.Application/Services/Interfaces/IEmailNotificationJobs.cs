namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IEmailNotificationJobs
    {
        Task SendOrderConfirmationAsync(int orderId);
        Task SendShippingConfirmationAsync(int orderId);
        Task SendDeliveryReminderAsync(int orderId);
        Task SendReviewReminderAsync(int orderId);
        Task SendCartReminderAsync(string userId);
    }
}
