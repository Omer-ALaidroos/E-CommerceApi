using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.DTOs.Order;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.command
{
    public class GetUserOrdersQueryHandler
        : IRequestHandler<GetUserOrdersQuery, List<OrderDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserOrdersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(
            GetUserOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,

                    ShippingAddress = _context.Addresses
                        .Where(a => a.Id == o.ShippingAddressId)
                        .Select(a => new ShippingAddressDto
                        {
                            Id = a.Id,
                            Street = a.Street,
                            City = a.City,
                            Country = a.Country
                        })
                        .First(),

                    Items = o.OrderItems
                        .Select(oi => new OrderItemDto
                        {
                            Id = oi.Id,
                            Quantity = oi.Quantity,
                            Price = oi.Price,

                            Product = new GetProduct
                            {
                                Id = oi.Product.Id,
                                Name = oi.Product.Name,
                                Description = oi.Product.Description,
                                Price = oi.Product.Price,
                                PrimaryImageUrl = oi.Product.Images.FirstOrDefault(pi => pi.IsPrimary).ImageUrl
                            }
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}