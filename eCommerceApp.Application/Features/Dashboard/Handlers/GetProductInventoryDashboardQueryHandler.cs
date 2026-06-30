using eCommerceApp.Application.DTOs.Dashboard;
using eCommerceApp.Application.Features.Dashboard.Queries;
using eCommerceApp.Application.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Application.Features.Dashboard.Handlers
{
    public class GetProductInventoryDashboardQueryHandler : IRequestHandler<GetProductInventoryDashboardQuery, ProductInventoryDashboardDto>
    {
        private readonly IApplicationDbContext _context;

        public GetProductInventoryDashboardQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductInventoryDashboardDto> Handle(
    GetProductInventoryDashboardQuery request,
    CancellationToken cancellationToken)
        {
            var totalProductsTask =await _context.Products
                .AsNoTracking()
                .CountAsync(p => !p.IsDeleted, cancellationToken);

            var totalCategoriesTask =await _context.Categories
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var topSellingProductsTask =await _context.OrderItems
                .AsNoTracking()
                .Where(oi =>
                    oi.Order.Status == OrderStatus.Delivered &&
                    !oi.Product.IsDeleted)
                .GroupBy(oi => new
                {
                    oi.ProductId,
                    oi.Product.Name,
                    oi.Product.ImageUrl
                })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? string.Empty,
                    ImageUrl = g.Key.ImageUrl,
                    TotalSoldQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                })
                .OrderByDescending(x => x.TotalSoldQuantity)
                .Take(5)
                .ToListAsync(cancellationToken);

            var lowStockProductsTask =await _context.Products
                .AsNoTracking()
                .Where(p =>
                    !p.IsDeleted &&
                    p.Quantity > 0 &&
                    p.Quantity < request.LowStockThreshold)
                .OrderBy(p => p.Quantity)
                .Select(p => new LowStockProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name ?? string.Empty,
                    ImageUrl = p.ImageUrl,
                    CurrentStock = p.Quantity,
                    CategoryName = p.category != null
                        ? p.category.Name ?? string.Empty
                        : string.Empty
                })
                .ToListAsync(cancellationToken);

        

            var topSellingProducts = topSellingProductsTask
                .Select((product, index) =>
                {
                    product.Rank = index + 1;
                    return product;
                })
                .ToList();

            return new ProductInventoryDashboardDto
            {
                TotalProducts = totalProductsTask,
                TotalCategories = totalCategoriesTask,
                TopSellingProducts = topSellingProducts,
                LowStockProducts = lowStockProductsTask
            };
        }
    }

   
}
