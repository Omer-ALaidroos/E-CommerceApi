using eCommerceApp.Application.Exceptions;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class ProductRepository(AppDbContext context) : IProduct
    {
        public async Task<int> AddAsync(Product entity)
        {
            await context.Set<Product>().AddAsync(entity);
            await context.SaveChangesAsync();


            var idProperty = typeof(Product).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Product does not have an 'Id' property.");

            var idValue = idProperty.GetValue(entity);
            return idValue is int id ? id : throw new InvalidOperationException("Id property is not of type int.");
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await context.Set<Product>().FindAsync(id);

            if (entity is null)
                return 0;

            context.Set<Product>().Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Set<Product>().AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var resukt = await context.Set<Product>().FindAsync(id) ??
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            return resukt;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var Products = await context.Products
                   .Include(p => p.category)
                  .Where(p => p.CategoryId == categoryId)
                  .AsNoTracking()
                  .ToListAsync();

            return Products.Count() > 0 ? Products : [];
        }

        public async Task<int> UpdateAsync(Product entity)
        {
            context.Set<Product>().Update(entity);
            return await context.SaveChangesAsync();
        }
    }
}
