using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Domain.Entities.Cart
{
    public class Achieve
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public string? UserId { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;


    }
}
