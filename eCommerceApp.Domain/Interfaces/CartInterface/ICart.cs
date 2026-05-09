using eCommerceApp.Domain.Entities.CartEntities;

namespace eCommerceApp.Domain.Interfaces.CartInterface
{
    public interface ICart
    {
        Task<int> SaveCheckoutHistory(IEnumerable<Achieve> checkOuts);
        Task<IEnumerable<Achieve>> GetAllCheckoutHistory();
        Task<int> RemoveCartItem(int itemId);
        Task<int> IncrementCartItemQuantity(int itemId);
        Task<int> DecrementCartItemQuantity(int itemId);
        Task<Cart?> GetActiveCart(string userId);
        Task<Cart> CreateCart(Cart cart);

        Task AddCartItem(CartItem item);
        Task UpdateCartItem(CartItem item);

        Task SaveChanges();
    }
}
