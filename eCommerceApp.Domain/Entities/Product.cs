﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceApp.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
      

        public int Quantity { get; set; }
        public Category? category { get; set; }

        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public double AverageRating { get; set; } = 0.0;

        public  int  ReviewsCount { get; set; } = 0;
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>(); // Already correct
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
