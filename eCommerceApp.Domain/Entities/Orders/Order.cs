using eCommerceApp.Domain.Entities.Identity;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required OrderStatus Status { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public required int ShippingAddressId { get; set; }

        public required int PaymentMethodId { get; set; }
        
        // Navigation Properties
        public required AppUser? User { get; set; }
       
        public required ICollection<OrderItem> OrderItems { get; set; }
    }
}
