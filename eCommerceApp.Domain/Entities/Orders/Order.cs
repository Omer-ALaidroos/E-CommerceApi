using ECommerce.Core.Entities;
using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Domain.Entities.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public required int ShippingAddressId { get; set; }

        public required int PaymentMethodId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? PaymentStatus { get; set; }

      
        public Address? ShippingAddress { get; set; }

        
        public PaymentMethod? PaymentMethod { get; set; }

        public AppUser? User { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
