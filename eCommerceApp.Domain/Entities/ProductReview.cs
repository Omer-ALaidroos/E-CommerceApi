using ECommerce.Core.Entities;
using eCommerceApp.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Domain.Entities
{
    public class ProductReview
    {
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// Order that contains this product (Optional)
        /// </summary>
        public int OrderItemId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties

        public Product Product { get; set; } = null!;

        public AppUser User { get; set; } = null!;
        public OrderItem OrderItem { get; set; } = null!;
    }
}
