using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.DTOs.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public GetProduct Product { get; set; }
    }
}
