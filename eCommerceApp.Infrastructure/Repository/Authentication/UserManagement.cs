using System.Security.Claims;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository.Authentication
{
#pragma warning disable CS9113 // Parameter is unread.
    public class UserManagement(IRoleManagement roleManagement,AppDbContext context,UserManager<AppUser> userManager) : IUserManagement
    {
       

        public async Task<bool> CreateUser(AppUser user)
        {
            AppUser? _user = await GetUserByEmail(user.Email!);
            if(_user != null) return false;
            return  (await userManager.CreateAsync(user ,user.PasswordHash!)).Succeeded;
        }

       

        public async Task<AppUser?> GetUserByEmail(string email) => await userManager.FindByEmailAsync(email);

        public async Task<AppUser> GetUserById(string userId)
        {
            AppUser? user = await userManager.FindByIdAsync(userId);
            return user!;
        }

        public async Task<IEnumerable<AppUser?>> GetAllUsers()=> await context.Users.ToListAsync();
       
        public async Task<int> RemoveUserByEmail(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                context.Users.Remove(user);
                return await context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<List<Claim>> GetUserClaims(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var roleName = await roleManagement.GetUserRole(user!.Email!);
            List<Claim> claims =
            [
                new Claim("FullName", user.FullName!),
                new Claim(ClaimTypes.NameIdentifier, user!.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, roleName!)
            ];
               
            return claims;
        }

      

        public async Task<bool> LoginUser(AppUser user)
        {
            var _user = await GetUserByEmail(user.Email!);
            if (_user == null) return false;

            string? roleName = await roleManagement.GetUserRole(_user.Email!);

            if (string.IsNullOrEmpty(roleName)) return false;

            return (await userManager.CheckPasswordAsync(_user, user.PasswordHash!));
        }
    }
}
