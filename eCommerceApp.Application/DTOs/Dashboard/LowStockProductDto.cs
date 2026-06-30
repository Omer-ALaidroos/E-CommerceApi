namespace eCommerceApp.Application.DTOs.Dashboard
{
    /// <summary>
    /// Represents a product with low stock in the dashboard.
    /// </summary>
    public class LowStockProductDto
    {
        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the current stock quantity of the product.
        /// </summary>
        public int CurrentStock { get; set; }

        /// <summary>
        /// Gets or sets the name of the category the product belongs to.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
    }
}
