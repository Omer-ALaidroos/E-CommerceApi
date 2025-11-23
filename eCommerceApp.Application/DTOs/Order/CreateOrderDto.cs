namespace ECommerce.Core.DTOs.Order
{
    public class CreateOrder
    {
        
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required OrderStatus Status { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public required int ShippingAddressId { get; set; }

        public required int PaymentMethodId { get; set; }
      
        
    }
}
