using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository
{
    public class AddressRepositroy(AppDbContext context) : IAddress
    {
        public async Task<int> AddAsync(Address entity)
        {
            await context.Set<Address>().AddAsync(entity);
            await context.SaveChangesAsync();


            var idProperty = typeof(Address).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Address does not have an 'Id' property.");

            var idValue = idProperty.GetValue(entity);
            return idValue is int id ? id : throw new InvalidOperationException("Id property is not of type int.");

        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await context.Set<Address>().FindAsync(id);

            if (entity is null)
                return 0;

            context.Set<Address>().Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await context.Set<Address>().AsNoTracking().ToListAsync();
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            Address entity = await context.Set<Address>().FindAsync(id);
            if (entity is null) return null;

            return entity;
        }

        public async Task<Address> GetUserAddressAsync(string UserId)
        {
            Address entity = await context.Set<Address>().Where(ad => ad.UserId == UserId).SingleOrDefaultAsync();
            if (entity is null) return null;

            return entity;
        }

        public async Task<int> UpdateAsync(Address entity)
        {
            context.Set<Address>().Update(entity);
            return await context.SaveChangesAsync();
        }
    }
}
