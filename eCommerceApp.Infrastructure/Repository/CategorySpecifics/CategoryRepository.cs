using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces.CategorySpecifics;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository.CategorySpecifics
{
    public class CategoryRepository(AppDbContext context) : ICategory
    {
        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var Products =await context.Products
                   .Include(p => p.category)
                  .Where(p => p.CategoryId == categoryId)
                  .AsNoTracking()
                  .ToListAsync(); 

          return Products.Count() > 0 ? Products : [];
        }
    }
}
