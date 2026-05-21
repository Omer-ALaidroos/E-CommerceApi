namespace ECommerce.Core.DTOs.Order
{
    public class CreateOrder
    {
        
        public required string UserID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public required int ShippingAddressId { get; set; }

        public required int PaymentMethodId { get; set; }
       
        public required decimal TotalAmount { get; set; }


    }
}
