using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCommerceApp.Domain.Interfaces
{
    public interface IGeneric<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(int id);
        Task<T> GetByIdAsync(int id);
    }
}