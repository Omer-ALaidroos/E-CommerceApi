using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();

        Task<GetProduct> GetByIdAsync(int id);
        Task<ServicesResponse> AddAsync(CreateProduct product);
        Task<ServicesResponse> UpdateAsync(UpdateProduct product);
        Task<ServicesResponse> DeleteAsync(int id);
    }

    
}
