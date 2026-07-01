using eCommerceApp.Application.DTOs.Dashboard;
using eCommerceApp.Application.Features.Dashboard.Queries;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace eCommerceApp.Application.Features.Dashboard.Handlers
{
    public class GetSalesAnalyticsQueryHandler : IRequestHandler<GetSalesAnalyticsQuery, SalesAnalyticsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetSalesAnalyticsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalesAnalyticsDto> Handle(GetSalesAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var (startDate, previousPeriodStartDate, previousPeriodEndDate) = GetDateRange(request.Period);

            var currentPeriodQuery = _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= startDate);

            var previousPeriodQuery = _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= previousPeriodStartDate && o.OrderDate < previousPeriodEndDate);

            // Perform aggregations in the database sequentially to ensure compatibility across all database providers.
            var totalRevenue = (double)(await currentPeriodQuery.Select(o => (decimal?)o.TotalAmount).SumAsync(cancellationToken) ?? 0);
            var totalOrders = await currentPeriodQuery.CountAsync(cancellationToken);

            var previousTotalRevenue = (double)(await previousPeriodQuery.Select(o => (decimal?)o.TotalAmount).SumAsync(cancellationToken) ?? 0);
            var previousTotalOrders = await previousPeriodQuery.CountAsync(cancellationToken); 

            var revenueGrowth = previousTotalRevenue > 0 ? ((totalRevenue - previousTotalRevenue) / previousTotalRevenue) * 100 : totalRevenue > 0 ? 100.0 : 0.0;
            var ordersGrowth = previousTotalOrders > 0 ? (((double)totalOrders - previousTotalOrders) / previousTotalOrders) * 100 : totalOrders > 0 ? 100.0 : 0.0;

            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
            var previousAverageOrderValue = previousTotalOrders > 0 ? previousTotalRevenue / previousTotalOrders : 0;
            var averageGrowth = previousAverageOrderValue > 0 ? ((averageOrderValue - previousAverageOrderValue) / previousAverageOrderValue) * 100 : averageOrderValue > 0 ? 100.0 : 0.0;

            // Also optimize the trend generation to be a database query
            var revenueTrend = await GenerateRevenueTrend(currentPeriodQuery, request.Period, cancellationToken);

            return new SalesAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                RevenueGrowth = revenueGrowth,
                TotalOrders = totalOrders,
                OrdersGrowth = ordersGrowth,
                AverageOrderValue = averageOrderValue,
                AverageGrowth = averageGrowth,
                RevenueTrend = revenueTrend
            };
        }

        private (DateTime startDate, DateTime previousPeriodStartDate, DateTime previousPeriodEndDate) GetDateRange(string period)
        {
            DateTime startDate;
            DateTime previousPeriodStartDate;
            DateTime previousPeriodEndDate;
            var now = DateTime.UtcNow;

            switch (period.ToLower())
            {
                case "week":
                    startDate = now.AddDays(-7);
                    previousPeriodStartDate = startDate.AddDays(-7);
                    previousPeriodEndDate = startDate;
                    break;
                case "30day":
                    startDate = now.AddDays(-30);
                    previousPeriodStartDate = startDate.AddDays(-30);
                    previousPeriodEndDate = startDate;
                    break;
                case "90day":
                    startDate = now.AddDays(-90);
                    previousPeriodStartDate = startDate.AddDays(-90);
                    previousPeriodEndDate = startDate;
                    break;
                case "year":
                    startDate = now.AddYears(-1);
                    previousPeriodStartDate = startDate.AddYears(-1);
                    previousPeriodEndDate = startDate;
                    break;
                default:
                    throw new ArgumentException("Invalid period specified. Valid periods are 'week', '30day', '90day', 'year'.");
            }
            return (startDate, previousPeriodStartDate, previousPeriodEndDate);
        }

        private async Task<List<RevenueTrendDto>> GenerateRevenueTrend(IQueryable<Domain.Entities.Orders.Order> ordersQuery, string period, CancellationToken cancellationToken)
        {
            if (period.ToLower() == "year")
            {
                return await ordersQuery
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .Select(g => new RevenueTrendDto
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Revenue = (double)g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync(cancellationToken);
            }
            else
            {
                // For other periods, group by day
                return await ordersQuery
                    .GroupBy(o => o.OrderDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new RevenueTrendDto
                    {
                        Date = g.Key.ToString("d MMM"),
                        Revenue = (double)g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
