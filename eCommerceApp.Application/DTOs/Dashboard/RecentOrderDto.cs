namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }
}
