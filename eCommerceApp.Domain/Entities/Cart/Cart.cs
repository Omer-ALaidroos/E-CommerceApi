using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Domain.Entities.Cart
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public bool IsCheckedOut { get; set; } 
        public DateTime CreatedAt { get; set; }

        public List<CartItem> Items { get; set; }
    }
}
