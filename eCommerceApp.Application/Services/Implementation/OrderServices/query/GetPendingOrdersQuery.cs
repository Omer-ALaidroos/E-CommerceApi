using eCommerceApp.Application.DTOs.Order;
using MediatR;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.query
{
    public record GetPendingOrdersQuery : IRequest<List<OrderDto?>>
    {
    }
    
}
