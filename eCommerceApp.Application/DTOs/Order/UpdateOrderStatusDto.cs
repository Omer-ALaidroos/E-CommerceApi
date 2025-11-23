namespace ECommerce.Core.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public int Id { get; set; }
        public required string Status { get; set; }
    }
}
