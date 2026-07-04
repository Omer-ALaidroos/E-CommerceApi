namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class TopCustomerDto
    {
        /// <summary>
        /// Gets or sets the customer's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the total amount spent by the customer.
        /// </summary>
        public decimal TotalSpent { get; set; }
        /// <summary>
        /// Gets or sets the customer's image URL.
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}