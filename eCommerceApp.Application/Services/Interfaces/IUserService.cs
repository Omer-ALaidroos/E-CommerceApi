using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUser?> GetByIdAsync(string id);
        Task<GetUser?> GetByEmailAsync(string email);
        Task<List<GetUser>> GetAllAsync();
        Task<List<GetUser>> SearchByNameAsync(string name);
        Task<List<GetUser>> SearchByEmailAsync(string email);
        Task<ServicesResponse> EditFullName(string fullName, string userId);
        Task<ServicesResponse> EditPhoneNumber(string phoneNumber, string userId);
        Task<ServicesResponse> SetUserAsAdmin(string email);
        Task<ServicesResponse> RemoveUserFromAdmin(string email);
    }
}
