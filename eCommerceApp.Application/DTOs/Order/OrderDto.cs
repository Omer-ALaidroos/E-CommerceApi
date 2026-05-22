using eCommerceApp.Application.DTOs.Address;

namespace eCommerceApp.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public ShippingAddressDto ShippingAddress { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }
}
