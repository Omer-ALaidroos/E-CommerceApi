using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Authentication;
using System.Text.RegularExpressions;

namespace eCommerceApp.Application.Services.Implementation
{
    public class UserService(IUserManagement userManagement, IMapper mapper) : IUserService
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

        public async Task<GetUser> GetByEmailAsync(string email)
        {
            var user = await userManagement.GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            var userDto = mapper.Map<GetUser>(user);
            return userDto;
        }

        public Task<GetUser> GetByIdAsync(string id)
        {
           
                var user =  userManagement.GetUserById(id).Result;
                if (user == null)
                {
                    return null;
                }
                var userDto = mapper.Map<GetUser>(user);
                return Task.FromResult(userDto);
        }
    }
}
