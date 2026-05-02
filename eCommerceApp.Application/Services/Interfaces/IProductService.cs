using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using Microsoft.AspNetCore.Http;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();
        Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(int categoryId);
        Task<GetProduct> GetByIdAsync(int id);
        Task<ServicesResponse> AddAsync(CreateProduct product,IFormFile formFile);
        Task<ServicesResponse> UpdateAsync(UpdateProduct product);
        Task<ServicesResponse> DeleteAsync(int id);

        
    }

    
}
