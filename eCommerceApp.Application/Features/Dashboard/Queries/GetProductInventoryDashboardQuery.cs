using MediatR;
using eCommerceApp.Application.DTOs.Dashboard;

namespace eCommerceApp.Application.Features.Dashboard.Queries
{
    /// <summary>
    /// Represents the query to get product and inventory dashboard data.
    /// </summary>
    public class GetProductInventoryDashboardQuery : IRequest<ProductInventoryDashboardDto>
    {
        /// <summary>
        /// Gets or sets the threshold for identifying low-stock products.
        /// Defaults to 10.
        /// </summary>
        public int LowStockThreshold { get; set; } = 10;
    }
}
