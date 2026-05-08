namespace eCommerceApp.Domain.Entities.CartEntities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public bool IsCheckedOut { get; set; } 
        public DateTime CreatedAt { get; set; }

        public List<CartItem>? Items { get; set; }
    }
}
