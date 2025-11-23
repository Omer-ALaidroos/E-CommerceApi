using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Domain.Interfaces.CategorySpecifics
{
    public interface ICategory
    {
        Task <IEnumerable<Product>> GetProductsByCategory(int categoryId);
    }
}