namespace eCommerceApp.Application.DTOs.Identity
{
    public record ResetPasswordDto(
        string Email,
        string Code, // The 6-digit OTP
        string NewPassword,
        string ConfirmPassword);
}