using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Domain.Entities.Cart
{
    public class Achieve
    {
        [Key]
        public int Id { get; set; } 
        public int OrderId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public string? UserId { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;


    }
}
