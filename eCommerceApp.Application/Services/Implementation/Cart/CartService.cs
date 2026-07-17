using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;

using eCommerceApp.Application.Services.Interfaces.CartInterface;
using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Domain.Interfaces.CartInterface;

class CartService(
	ICart cartInterface,
	IMapper mapper,
	IProduct ProductInterface,
	IPaymentMethodService paymentMethodService,
	IPaymentService paymentService,
	IUserManagement userManagement) : ICartService
{

	public async Task<ServicesResponse> AddToCart(string userId, int productId, int quantity)
	{
		if (quantity <= 0)
			return new ServicesResponse(false, "Invalid quantity");

		var product = await ProductInterface.GetByIdAsync(productId);
		if (product == null)
			return new ServicesResponse(false, "Product not found");

		var cart = await cartInterface.GetActiveCart(userId);

		if (cart == null)
		{
			cart = new Cart
			{
				UserId = userId,
				CreatedAt = DateTime.UtcNow,
				IsCheckedOut = false
			};

			await cartInterface.CreateCart(cart);
		}

		var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

		if (item != null)
		{
			item.Quantity += quantity;
			await cartInterface.UpdateCartItem(item);
		}
		else
		{
			await cartInterface.AddCartItem(new CartItem
			{
				Cart = cart,
				ProductId = productId,
				Quantity = quantity,
				PriceAtTime = product.Price
			});
		}

		await cartInterface.SaveChanges();

		return new ServicesResponse(true, "Added to cart");
	}

	public async Task<IEnumerable<GetCartDto>> GetMyCart(string userId)
	{
		var cart = await cartInterface.GetActiveCart(userId);

		if (cart == null || !cart.Items.Any())
			return Enumerable.Empty<GetCartDto>();

		var products = await ProductInterface.GetAllAsync();

		return cart.Items.Select(i =>
		{
			var p = products.FirstOrDefault(x => x.Id == i.ProductId);

			return new GetCartDto
			{
				CartItemId = i.Id,
                ProductId = i.ProductId,
				ProductName = p?.Name ?? "Unknown Product",
				Quantity = i.Quantity,
				Price = i.PriceAtTime,
				Total = i.Quantity * i.PriceAtTime,
				ImageUrl = p?.Images?.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl
			};
		});
	}

	
	public async Task<Cart?> GetActiveCart(string userId)
	{
		return await cartInterface.GetActiveCart(userId);
	}

	public async Task<ServicesResponse> Checkout(string userId, int paymentMethodId)
	{
		var cart = await GetActiveCart(userId);

		if (cart == null || !cart.Items.Any())
			return new ServicesResponse(false, "Cart is empty");

		var paymentMethods = await paymentMethodService.GetPaymntMethods();
		if (paymentMethods.All(p => p.Id != paymentMethodId))
			return new ServicesResponse(false, "Invalid Payment Method");

		var total = cart.Items.Sum(i => i.Quantity * i.PriceAtTime);

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await ProductInterface.GetByIdsAsync(productIds);

       var paymentResponse = await paymentService.Pay(total, cart.Items, products);
		if (!paymentResponse.IsSuccess)
			return paymentResponse;

		var achieves = cart.Items.Select(i => new Achieve
		{
			UserId = userId,
			productId = i.ProductId,
			quantity = i.Quantity,
			createdDate = DateTime.UtcNow
		});

		await cartInterface.SaveCheckoutHistory(achieves);

		cart.IsCheckedOut = true;

		await cartInterface.SaveChanges();

		return new ServicesResponse(true, "Checkout done");
	}

	public async Task<ServicesResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves)
	{
		var mappedData = mapper.Map<IEnumerable<Achieve>>(achieves);
		if (mappedData == null)
			return new ServicesResponse(false, "Mapping failed");

		var result = await cartInterface.SaveCheckoutHistory(mappedData);

		return result > 0
			? new ServicesResponse(true, "Saved")
			: new ServicesResponse(false, "Error");
	}

	public async Task<IEnumerable<GetAchieve>> GetAchieves()
	{
		var achieves = await cartInterface.GetAllCheckoutHistory();
		return mapper.Map<IEnumerable<GetAchieve>>(achieves);
	}

    public async Task<ServicesResponse> RemoveCartItem(int itemId)
    {
       int result = await cartInterface.RemoveCartItem(itemId);
		return result > 0
			? new ServicesResponse(true, "Item removed")
			: new ServicesResponse(false, "Error removing item");

    }

    public async Task<ServicesResponse> DecrementCartItemQuantity(int itemId)
    {
        int result = await cartInterface.DecrementCartItemQuantity(itemId);
		return result > 0
			? new ServicesResponse(true, "Quantity decremented")
			: new ServicesResponse(false, "Error decrementing quantity");
    }

    public async Task<ServicesResponse> IncrementCartItemQuantity(int itemId)
    {
        int result = await cartInterface.IncrementCartItemQuantity(itemId);
        return result > 0
            ? new ServicesResponse(true, "Quantity incremented")
            : new ServicesResponse(false, "Error incrementing quantity");
    }

  /*  public async Task<IEnumerable<ProcessCart>> GetCartItemsBuUserID(string userID)
    {
        var cartItems = await cartInterface.GetCartItems(userID);
        return cartItems.Select(ci => new ProcessCart
        {
            ProductId = ci.ProductId,
            Quantity = ci.Quantity
        });
    }*/

    public async Task ClearCartAsync(string userId)
    {
        var cart = await cartInterface.GetActiveCart(userId);
        if (cart != null)
        {
            cart.IsCheckedOut = true;
            await cartInterface.SaveChanges();
        }
    }

    
}