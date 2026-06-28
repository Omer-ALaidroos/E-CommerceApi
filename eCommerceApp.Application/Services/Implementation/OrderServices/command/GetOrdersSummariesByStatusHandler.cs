using eCommerceApp.Application.DTOs.Order;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.command
{
    public class GetOrdersSummariesByStatusHandler
          : IRequestHandler<GetOrdersSummariesByStatusQuery, List<OrderSummaryDto?>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdersSummariesByStatusHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderSummaryDto>> Handle(
       GetOrdersSummariesByStatusQuery request,
       CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status.ToString() == request.status)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}
