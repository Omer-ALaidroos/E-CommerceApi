using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Domain.Entities.CartEntities
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
