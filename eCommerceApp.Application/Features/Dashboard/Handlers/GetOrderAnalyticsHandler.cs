using eCommerceApp.Application.DTOs.Dashboard;
using eCommerceApp.Application.Features.Dashboard.Queries;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Features.Dashboard.Handlers
{
    public class GetOrderAnalyticsHandler : IRequestHandler<GetOrderAnalyticsQuery, OrderAnalyticsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetOrderAnalyticsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderAnalyticsDto> Handle(GetOrderAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var totalOrders = await _context.Orders.AsNoTracking().CountAsync(cancellationToken);

            var statusSummaryQuery = _context.Orders
                .AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                });

            var statusSummaries = await statusSummaryQuery.ToListAsync(cancellationToken);

            var statusSummaryDtos = statusSummaries.Select(s => new OrderStatusSummaryDto
            {
                Status = s.Status.ToString(),
                Count = s.Count,
                Percentage = totalOrders > 0 ? (double)s.Count / totalOrders * 100 : 0
            }).ToList();

            var recentOrders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.Id,
                    CustomerName = o.User != null ? (o.User.FullName ) : "N/A",
                    OrderDate = o.OrderDate
                })
                .ToListAsync(cancellationToken);

            var result = new OrderAnalyticsDto
            {
                TotalOrders = totalOrders,
                StatusSummary = statusSummaryDtos,
                RecentOrders = recentOrders
            };

            return result;
        }
    }
}