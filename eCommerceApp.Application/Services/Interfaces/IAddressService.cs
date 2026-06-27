using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Address;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<GetAddress>> GetAllAsync();

        Task<GetAddress> GetByIdAsync(int id);
        Task<ServicesResponse> AddAsync(CreateAddress address);
        Task<ServicesResponse> UpdateAsync(UpdateAddress address);
        Task<ServicesResponse> DeleteAsync(int id);

        Task<GetAddress> GetUserAddressAsync(string userId);
    }
}
