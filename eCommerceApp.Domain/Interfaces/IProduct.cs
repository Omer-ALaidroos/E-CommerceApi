using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IProduct
    {
        Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
        public Task<IEnumerable<Product>> GetAllAsync();
        Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
        public Task<Product> GetByIdAsync(int id);
        public Task<int> AddAsync(Product entity);
        public Task<int> UpdateAsync(Product entity);
        public Task<int> DeleteAsync(int id);
    }
}
