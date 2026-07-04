using System.Collections.Generic;

namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class OrderAnalyticsDto
    {
        public int TotalOrders { get; set; }
        public List<OrderStatusSummaryDto> StatusSummary { get; set; } = new List<OrderStatusSummaryDto>();
        public List<RecentOrderDto> RecentOrders { get; set; } = new List<RecentOrderDto>();
    }
}
