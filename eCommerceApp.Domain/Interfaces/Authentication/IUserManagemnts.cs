using eCommerceApp.Domain.Entities.Identity;
using System.Security.Claims;

namespace eCommerceApp.Domain.Interfaces.Authentication
{
    public interface IUserManagement
    {
        Task<bool> CreateUser(AppUser user);
        Task<bool> LoginUser(AppUser user);
       Task<AppUser?> GetUserByEmail(string email);
        Task<AppUser> GetUserById(string userId);

        Task<IEnumerable<AppUser?>> GetAllUsers();
        Task<int> RemoveUserByEmail(string email);
        Task<List<Claim>> GetUserClaims(string email);

        Task<int> EditFullName(string fullName, string userId);
        Task<int> EditPhoneNumber(string phoneNumber, string userId);
        Task<int> ChangePassword( string newPassword, string userId);
    }
}
