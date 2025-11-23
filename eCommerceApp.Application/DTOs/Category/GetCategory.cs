using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.DTOs.Category
{
    public class GetCategory :CategoryBase
    {
        
        public int Id { get; set; }

        public ICollection<GetProduct>? Products { get; set; }
    }
}