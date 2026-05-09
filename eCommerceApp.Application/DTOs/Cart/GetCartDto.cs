namespace eCommerceApp.Application.DTOs.Cart
{
    public class GetCartDto
    { 
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }

        public string? ImageUrl { get; set; }
    }
}
