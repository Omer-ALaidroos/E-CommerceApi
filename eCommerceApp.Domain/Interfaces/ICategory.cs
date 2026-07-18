using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces
{
    public interface ICategory
    {
        Task <IEnumerable<Product>> GetProductsByCategory(int categoryId);
        public Task<IEnumerable<Category>> GetAllAsync();

        public Task<Category> GetByIdAsync(int id);
        public Task<int> AddAsync(Category entity);
        public Task<int> UpdateAsync(Category entity);
        public Task<int> DeleteAsync(int id);
    }
}