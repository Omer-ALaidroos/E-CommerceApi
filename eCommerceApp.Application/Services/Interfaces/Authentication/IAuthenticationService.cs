using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;

namespace eCommerceApp.Application.Services.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<ServicesResponse> CreateUser(CreateUser user);
        Task<ServicesResponse> ChangePassword(ChangePassword changePassword, string userId);
        Task<LoginResponse> LoginUser(LoginUser user);
        Task<LoginResponse> ReviveToken(string RefreshToken);
        Task<ServicesResponse> ForgotPassword(ForgotPasswordDto model);
        Task<ServicesResponse> VerifyResetCode(VerifyResetCodeDto model);
        Task<ServicesResponse> ResetPassword(ResetPasswordDto model);
    }
}
