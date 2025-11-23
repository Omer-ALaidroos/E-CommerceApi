using eCommerceApp.Application.Exceptions;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class GenericRepository<TEntity>(AppDbContext context) : IGeneric<TEntity> where TEntity : class
    {
       

        public async Task<int> AddAsync(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity);
            return await context.SaveChangesAsync() ;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity =await context.Set<TEntity>().FindAsync(id);

            if (entity is null)
                return 0;

            context.Set<TEntity>().Remove(entity);
            return await context.SaveChangesAsync();
        }

       

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           return await context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var resukt = await context.Set<TEntity>().FindAsync(id) ??
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            return resukt;



        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
           TEntity entity = await context.Set<TEntity>().FindAsync( id);
            if (entity is null) return null;

            return entity;
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
            return await context.SaveChangesAsync();
        }
    }
}
