namespace eCommerceApp.Domain.Interfaces
{
    public interface IAddress
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<int> AddAsync(Address entity);
        Task<int> UpdateAsync(Address entity);
        Task<int> DeleteAsync(int id);
        Task<Address> GetByIdAsync(int id);
        Task<Address> GetUserAddressAsync(string UserId);
    }
}
