using System;

namespace eCommerceApp.Domain.Entities.Identity
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptsCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}