using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Interfaces.Authentication;
using System.Linq;
using System.Text.RegularExpressions;

namespace eCommerceApp.Application.Services.Implementation
{
    public class UserService(IUserManagement userManagement, IMapper mapper, IRoleManagement roleManagement) : IUserService
    {
        public async Task<ServicesResponse> EditFullName(string fullName, string userId)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return new ServicesResponse(false, "Full name cannot be empty.");
            }

           int result=  await userManagement.EditFullName(fullName, userId);
         

            return result > 0
                ? new ServicesResponse(true, "Full name updated successfully.")
                : new ServicesResponse(false, "Failed to update full name.");
        }

        public async Task<ServicesResponse> EditPhoneNumber(string phoneNumber, string userId)
        {
          
            if (string.IsNullOrEmpty(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^(77|71|73|78)\d{7}$"))
            {
                return new ServicesResponse(false, "Phone number must start with 77, 71, 73, or 78 and be 9 digits long.");
            }

            int result = await userManagement.EditPhoneNumber(phoneNumber, userId);

            return result > 0
                ? new ServicesResponse(true, "Phone number updated successfully.")
                : new ServicesResponse(false, "Failed to update phone number.");
        }

        public async Task<GetUser?> GetByEmailAsync(string email)
        {
            var user = await userManagement.GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            var userDto = mapper.Map<GetUser>(user);
            return userDto;
        }

        public async Task<GetUser?> GetByIdAsync(string id)
        {
            var user = await userManagement.GetUserById(id);
            if (user == null)
            {
                return null;
            }
            var userDto = mapper.Map<GetUser>(user);
            return userDto;
        }

        public async Task<List<GetUser>> GetAllAsync()
        {
            var users = await userManagement.GetAllUsers();
            var userDtos = mapper.Map<List<GetUser>>(users);

            foreach (var user in users)
            {
                var role = await roleManagement.GetUserRole(user.Email!);
                var dto = userDtos.FirstOrDefault(u => u.Email == user.Email);
                if (dto != null)
                {
                    dto.Role = role ?? string.Empty;
                }
            }

            return userDtos;
        }

        public async Task<List<GetUser>> SearchByNameAsync(string name)
        {
            var users = await userManagement.SearchUsersByName(name);
            var userDtos = mapper.Map<List<GetUser>>(users);

            foreach (var user in users)
            {
                var role = await roleManagement.GetUserRole(user!.Email!);
                var dto = userDtos.FirstOrDefault(u => u.Email == user!.Email);
                if (dto != null)
                {
                    dto.Role = role ?? string.Empty;
                }
            }

            return userDtos;
        }

        public async Task<List<GetUser>> SearchByEmailAsync(string email)
        {
            var users = await userManagement.SearchUsersByEmail(email);
            var userDtos = mapper.Map<List<GetUser>>(users);

            foreach (var user in users)
            {
                var role = await roleManagement.GetUserRole(user!.Email!);
                var dto = userDtos.FirstOrDefault(u => u.Email == user!.Email);
                if (dto != null)
                {
                    dto.Role = role ?? string.Empty;
                }
            }

            return userDtos;
        }

        public async Task<ServicesResponse> SetUserAsAdmin(string email)
        {
            var user = await userManagement.GetUserByEmail(email);
            if (user == null)
            {
                return new ServicesResponse(false, "User not found.");
            }

            var result = await roleManagement.AddUserToRole(user, "Admin");
            return result
                ? new ServicesResponse(true, "User assigned to Admin role.")
                : new ServicesResponse(false, "Failed to assign Admin role.");
        }

        public async Task<ServicesResponse> RemoveUserFromAdmin(string email)
        {
            var user = await userManagement.GetUserByEmail(email);
            if (user == null)
            {
                return new ServicesResponse(false, "User not found.");
            }

            var result = await roleManagement.RemoveUserFromRole(user, "Admin");
            return result
                ? new ServicesResponse(true, "User removed from Admin role.")
                : new ServicesResponse(false, "Failed to remove Admin role.");
        }
    }
}
