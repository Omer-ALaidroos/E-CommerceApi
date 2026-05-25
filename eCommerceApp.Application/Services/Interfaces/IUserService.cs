using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUser> GetByIdAsync(string id);
            Task<GetUser> GetByEmailAsync(string email);
        Task<ServicesResponse> EditFullName(string fullName, string userId);
        Task<ServicesResponse> EditPhoneNumber(string phoneNumber, string userId);
    }
}
