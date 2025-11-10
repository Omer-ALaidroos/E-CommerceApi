using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<IEnumerable<GetCategory>> GetAllAsync();

        public Task<GetCategory> GetByIdAsync(Guid id);
        public Task<ServicesResponse> AddAsync(CreateCategory category);
        public Task<ServicesResponse> UpdateAsync(UpdateCategory category);
        public Task<ServicesResponse> DeleteAsync(Guid id);
        Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(Guid categoryId);

    }
}
