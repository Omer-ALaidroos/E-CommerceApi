using System.Collections.Generic;

namespace eCommerceApp.Application.DTOs.Dashboard
{
    public class CustomerAnalyticsDto
    {
        /// <summary>
        /// Gets or sets the total number of customers.
        /// </summary>
        public int TotalCustomers { get; set; }
        /// <summary>
        /// Gets or sets the number of new customers in the last 30 days.
        /// </summary>
        public int NewCustomersLast30Days { get; set; }
        /// <summary>
        /// Gets or sets the list of top spending customers.
        /// </summary>
        public ICollection<TopCustomerDto> TopCustomers { get; set; } = new List<TopCustomerDto>();
    }
}