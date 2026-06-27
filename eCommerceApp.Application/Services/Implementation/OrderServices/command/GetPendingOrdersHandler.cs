using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.DTOs.Order;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Application.Services.Implementation.OrderServices.command
{
    public class GetPendingOrdersHandler
          : IRequestHandler<GetPendingOrdersQuery, List<OrderDto?>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingOrdersHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto?>> Handle(GetPendingOrdersQuery request, CancellationToken cancellationToken)
        {
            var PendingOrders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Pending)
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
                        .FirstOrDefault()!,
                    Items = o.OrderItems != null
                        ? o.OrderItems
                            .Select(oi => new OrderItemDto
                            {
                                Id = oi.Id,
                                Quantity = oi.Quantity,
                                Price = oi.Price,
                                Product = new GetProduct
                                {
                                    Id = oi.Product!.Id,
                                    Name = oi.Product.Name,
                                    Description = oi.Product.Description,
                                    Price = oi.Product.Price,
                                    ImageUrl = oi.Product.ImageUrl
                                }
                            })
                            .ToList()
                        : new List<OrderItemDto>()
                }).ToListAsync(cancellationToken);

            return PendingOrders;
        }
    }
}
