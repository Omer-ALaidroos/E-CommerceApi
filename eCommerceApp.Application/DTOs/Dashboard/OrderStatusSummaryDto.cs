namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class OrderStatusSummaryDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
