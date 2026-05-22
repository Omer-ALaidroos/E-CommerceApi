using eCommerceApp.Application.DTOs.Order;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetUserOrderSummariesQueryHandler
    : IRequestHandler<GetUserOrderSummariesQuery, List<OrderSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserOrderSummariesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderSummaryDto>> Handle(
        GetUserOrderSummariesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId)
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