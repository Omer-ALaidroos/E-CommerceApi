using eCommerceApp.Application.DTOs.Address;

namespace ECommerce.Core.DTOs.Order
{
    // It's better to have a specific DTO for order items as well
   

    public class GetOrder
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int PaymentMethodId { get; set; }
        public required GetAddress ShippingAddress { get; set; } 
    }
}
