using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository.Authentication
{
    public class PasswordResetOtpRepository(AppDbContext context) : IPasswordResetOtpRepository
    {
        public async Task AddAsync(PasswordResetOtp otp)
        {
            context.PasswordResetOtps.Add(otp);
            await context.SaveChangesAsync();
        }

        public async Task<PasswordResetOtp?> GetActiveOtpAsync(string userId, string code)
        {
            return await context.PasswordResetOtps
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Code == code && !o.IsUsed);
        }

        public async Task<PasswordResetOtp?> GetLatestActiveOtpAsync(string userId)
        {
            return await context.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidateOldOtpsAsync(string userId)
        {
            var existingOtps = await context.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.IsUsed)
                .ToListAsync();

            foreach (var otp in existingOtps) otp.IsUsed = true;
            
            if (existingOtps.Any()) await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PasswordResetOtp otp)
        {
            context.PasswordResetOtps.Update(otp);
            await context.SaveChangesAsync();
        }
    }
}