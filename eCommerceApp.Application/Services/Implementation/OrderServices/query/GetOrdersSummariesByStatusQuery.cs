using eCommerceApp.Application.DTOs.Order;
using MediatR;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.query
{
    public record GetOrdersSummariesByStatusQuery(string status) : IRequest<List<OrderSummaryDto?>>;
    
}
