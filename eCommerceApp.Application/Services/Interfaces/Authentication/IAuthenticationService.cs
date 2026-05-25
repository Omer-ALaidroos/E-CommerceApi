using eCommerceApp.Application.DTOs;

namespace eCommerceApp.Application.Services.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<ServicesResponse> CreateUser(CreateUser user);
        Task<ServicesResponse> ChangePassword(ChangePassword changePassword, string userId);
        Task<LoginResponse> LoginUser(LoginUser user);
        Task<LoginResponse> ReviveToken(string RefreshToken);
    }
}
