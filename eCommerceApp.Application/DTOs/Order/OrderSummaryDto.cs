namespace eCommerceApp.Application.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }
    }
}
