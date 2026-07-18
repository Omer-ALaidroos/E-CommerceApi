namespace eCommerceApp.Application.DTOs.Dashboard
{
    /// <summary>
    /// Represents the response DTO for the product and inventory dashboard.
    /// </summary>
    public class ProductInventoryDashboardDto
    {
        /// <summary>
        /// Gets or sets the total number of active products.
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Gets or sets the total number of categories.
        /// </summary>
        public int TotalCategories { get; set; }

        /// <summary>
        /// Gets or sets the list of top-selling products.
        /// </summary>
        public ICollection<TopSellingProductDto> TopSellingProducts { get; set; } = new List<TopSellingProductDto>();

        /// <summary>
        /// Gets or sets the list of products with low stock.
        /// </summary>
        public ICollection<LowStockProductDto> LowStockProducts { get; set; } = new List<LowStockProductDto>();
    }
}
