using eCommerceApp.Application.Exceptions;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class ProductRepository(AppDbContext context) : IProduct
    {
         public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
        public async Task AddAsync(Product entity)
        {
            await context.Set<Product>().AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await context.Set<Product>().FindAsync(id);

            if (entity is null)
                return; // Or throw an exception if deletion of non-existent item is an error
            entity.IsDeleted = true;
          
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Set<Product>()
                .Where(p => p.Quantity > 0 && p.IsDeleted == false)
                .AsNoTracking().ToListAsync();
        }
       
        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await context.Set<Product>()
                .Where(p => p.Quantity > 0 && p.Quantity > 0 && p.IsDeleted == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var result = await context.Set<Product>().FindAsync(id) ??
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            return result;
        }
        
        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var Products = await context.Products
                   .Include(p => p.category)
                  .Where(p => p.CategoryId == categoryId && p.Quantity > 0 && p.IsDeleted == false)
                  .AsNoTracking()
                  .ToListAsync();

            return Products.Count() > 0 ? Products : [];
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsByCategoryAsync(int categoryId)
        {
            var products = await context.Products
                .Include(p => p.category)
                .Where(p => p.CategoryId == categoryId && p.Quantity > 0 && p.IsDeleted == false)
                .AsNoTracking()
                .ToListAsync();
            return products.Count() > 0 ? products : [];
        }

        public Task UpdateAsync(Product entity)
        {
            context.Set<Product>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task DecreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null || product.Quantity < quantity)
                return;

            product.Quantity -= quantity;
        }

        public async Task IncreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null)
                return ;
            product.Quantity += quantity;
            
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
            
        }
    }


}
