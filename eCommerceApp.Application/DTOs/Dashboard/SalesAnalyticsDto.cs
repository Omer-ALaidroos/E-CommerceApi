namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class SalesAnalyticsDto
    {
        public double TotalRevenue { get; set; }
        public double RevenueGrowth { get; set; }
        public int TotalOrders { get; set; }
        public double OrdersGrowth { get; set; }
        public double AverageOrderValue { get; set; }
        public double AverageGrowth { get; set; }
        public List<RevenueTrendDto> RevenueTrend { get; set; }
    }
}
