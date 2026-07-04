﻿using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IProduct
    {
        Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
        public Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
        public Task<Product> GetByIdAsync(int id);
        public Task AddAsync(Product entity); // Changed return type to Task
        public Task UpdateAsync(Product entity); // Changed return type to Task
        public Task DeleteAsync(int id); // Changed return type to Task
        Task DecreaseProductQuantityAsync(int productId, int quantity);
        Task IncreaseProductQuantityAsync(int productId, int quantity);
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<IEnumerable<Product>> GetAvailableProductsByCategoryAsync(int categoryId);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
    }
}
