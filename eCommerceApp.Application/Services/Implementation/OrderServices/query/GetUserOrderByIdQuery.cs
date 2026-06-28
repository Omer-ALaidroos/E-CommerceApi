using eCommerceApp.Application.DTOs.Order;
using MediatR;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.query
{
   

    public record GetUserOrderByIdQuery(
        int OrderId
       )
        : IRequest<OrderDto?>;
}
