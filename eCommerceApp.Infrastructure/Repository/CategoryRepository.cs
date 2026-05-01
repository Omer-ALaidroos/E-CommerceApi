using eCommerceApp.Application.Exceptions;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class CategoryRepository(AppDbContext context) : ICategory
    {
        public async Task<int> AddAsync(Category entity)
        {
            await context.Set<Category>().AddAsync(entity);
            await context.SaveChangesAsync();

           
            var idProperty = typeof(Category).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity does not have an 'Id' property.");

            var idValue = idProperty.GetValue(entity);
            return idValue is int id ? id : throw new InvalidOperationException("Id property is not of type int.");
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await context.Set<Category>().FindAsync(id);

            if (entity is null)
                return 0;

            context.Set<Category>().Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await context.Set<Category>().AsNoTracking().ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var resukt = await context.Set<Category>().FindAsync(id) ??
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            return resukt;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var Products =await context.Products
                   .Include(p => p.category)
                  .Where(p => p.CategoryId == categoryId)
                  .AsNoTracking()
                  .ToListAsync(); 

          return Products.Count() > 0 ? Products : [];
        }

        public async Task<int> UpdateAsync(Category entity)
        {
            context.Set<Category>().Update(entity);
            return await context.SaveChangesAsync();
        }
    }
}
