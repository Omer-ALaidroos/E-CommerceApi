using MediatR;
using eCommerceApp.Application.DTOs.Dashboard;

namespace eCommerceApp.Application.Features.Dashboard.Queries
{
    public class GetSalesAnalyticsQuery : IRequest<SalesAnalyticsDto>
    {
        public string Period { get; set; } = "week";
    }
}
