namespace eCommerceApp.Domain.Interfaces
{
    public interface IGeneric<IEntity> where IEntity : class
    {
        public Task<IEnumerable<IEntity>> GetAllAsync();

        public Task<IEntity> GetByIdAsync(Guid id);
        public Task<int> AddAsync(IEntity entity);
        public Task<int> UpdateAsync(IEntity entity);
        public Task<int> DeleteAsync(Guid id);


    }
    
}
