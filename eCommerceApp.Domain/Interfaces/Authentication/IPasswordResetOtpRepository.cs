using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Domain.Interfaces.Authentication
{
    public interface IPasswordResetOtpRepository
    {
        Task<PasswordResetOtp?> GetActiveOtpAsync(string userId, string code);
        Task<PasswordResetOtp?> GetLatestActiveOtpAsync(string userId);
        Task InvalidateOldOtpsAsync(string userId);
        Task AddAsync(PasswordResetOtp otp);
        Task UpdateAsync(PasswordResetOtp otp);
    }
}