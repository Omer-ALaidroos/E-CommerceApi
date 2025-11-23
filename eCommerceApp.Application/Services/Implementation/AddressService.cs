using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementation
{
    public class AddressService(IGeneric<Address> AddressInterface, IMapper mapper) : IAddressService
    {
        public async Task<ServicesResponse> AddAsync(CreateAddress address)
        {
            Address mappedAddress = mapper.Map<Address>(address);
            int result = await AddressInterface.AddAsync(mappedAddress);

            if (result > 0)
            {
                return new ServicesResponse(true, "Address added successfully.");

            }
            else
            {
                return new ServicesResponse(false, "Failed to add Address.");

            }
        }

        public async  Task<ServicesResponse> DeleteAsync(int id)
        {
            int result = await AddressInterface.DeleteAsync(id);




            return result > 0 ?
                new ServicesResponse(true, "Address delete successfully.") :
                new ServicesResponse(false, "Failed to delete Address."); ;
        }

        public async Task<IEnumerable<GetAddress>> GetAllAsync()
        {
            var Addresses = await AddressInterface.GetAllAsync();

            if (!Addresses.Any()) return [];

            return mapper.Map<IEnumerable<GetAddress>>(Addresses);
        }

        public async  Task<GetAddress> GetByIdAsync(int id)
        {
            var Address = await AddressInterface.GetByIdAsync(id);

            if (Address == null) return null;

            return mapper.Map<GetAddress>(Address);
        }

        public async Task<ServicesResponse> UpdateAsync(UpdateAddress address)
        {
            var mappedAddress = mapper.Map<Address>(address);
            int result = await AddressInterface.UpdateAsync(mappedAddress);
            if (result > 0)
            {
                return new ServicesResponse(true, "Address updated successfully.");
            }
            else
            {
                return new ServicesResponse(false, "Failed to update Address.");
            }
        }
    }
}
