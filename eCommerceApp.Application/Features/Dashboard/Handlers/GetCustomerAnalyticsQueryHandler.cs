using eCommerceApp.Application.DTOs.Dashboard;
using eCommerceApp.Application.Features.Dashboard.Queries;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Features.Dashboard.Handlers
{
    public class GetCustomerAnalyticsQueryHandler : IRequestHandler<GetCustomerAnalyticsQuery, CustomerAnalyticsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetCustomerAnalyticsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerAnalyticsDto> Handle(GetCustomerAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var totalCustomersTask =await _context.Users.AsNoTracking().CountAsync(cancellationToken);

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var newCustomersLast30DaysTask =await _context.Users
                .AsNoTracking()
                .CountAsync(u => u.CreatedDate >= thirtyDaysAgo, cancellationToken);

            var topCustomersTask =await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Delivered && o.User != null)
                .GroupBy(o => new { o.UserId, o.User.FullName, o.User.ImageUrl })
                .Select(g => new TopCustomerDto
                {
                    Name = g.Key.FullName,
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    ImageUrl = g.Key.ImageUrl
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToListAsync(cancellationToken);

           


            var result = new CustomerAnalyticsDto
            {
                TotalCustomers = totalCustomersTask,
                NewCustomersLast30Days = newCustomersLast30DaysTask,
                TopCustomers = topCustomersTask
            };

            return result;
        }
    }
}