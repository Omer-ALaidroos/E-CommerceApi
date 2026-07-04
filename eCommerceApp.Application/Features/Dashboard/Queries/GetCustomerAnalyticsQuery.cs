using eCommerceApp.Application.DTOs.Dashboard;
using MediatR;

namespace eCommerceApp.Application.Features.Dashboard.Queries
{
    public class GetCustomerAnalyticsQuery : IRequest<CustomerAnalyticsDto>
    {
    }
}