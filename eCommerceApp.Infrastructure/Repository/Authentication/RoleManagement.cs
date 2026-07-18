using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;

namespace eCommerceApp.Infrastructure.Repository.Authentication
{
#pragma warning disable CS9113 // Parameter is unread.
    public class RoleManagement(UserManager<AppUser> UserManager) : IRoleManagement

    {
        public async Task<bool> AddUserToRole(AppUser user, string roleName) =>
        (await UserManager.AddToRoleAsync(user, roleName)).Succeeded;

        public async Task<string?> GetUserRole(string useremail)
        {
            var user = await UserManager.FindByEmailAsync(useremail);
          return (await UserManager.GetRolesAsync(user!)).FirstOrDefault();
        }

        public async Task<bool> RemoveUserFromRole(AppUser user, string roleName) =>
            (await UserManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
    }
}
