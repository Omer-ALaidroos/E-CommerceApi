using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.CartInterface;

namespace eCommerceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpPost("add")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var result = await cartService.AddToCart(userId, dto.ProductId, dto.Quantity);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-cart")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetMyCart()
        {
            var userId = GetUserId();

            var cart = await cartService.GetMyCart(userId);

            return  Ok(cart) ;
        }

        [HttpDelete("remove/{cartItem:int}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> RemoveCartItem(int cartITem)
        {
            var userId = GetUserId();

            var result = await cartService.RemoveCartItem( cartITem);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("decrement-quentitey")]
        [Authorize(Roles ="User")]
        public async Task<ActionResult> DecrementCartItemQuantity([FromQuery] int cartItemId)
        {
            var userId = GetUserId();
            var result = await cartService.DecrementCartItemQuantity(cartItemId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("increment-quentitey")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> IncrementCartItemQuantity([FromQuery] int cartItemId)
        {
            var userId = GetUserId();
            var result = await cartService.IncrementCartItemQuantity(cartItemId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        /*[HttpPost("checkout")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Checkout([FromQuery] int paymentMethodId)
        {
            var userId = GetUserId();

            var result = await cartService.Checkout(userId, paymentMethodId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("save-checkout")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> SaveCheckoutHistory([FromBody] IEnumerable<CreateAchieve> achieves)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await cartService.SaveCheckoutHistory(achieves);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-achieves")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllCheckoutHistory()
        {
            var achieves = await cartService.GetAchieves();

            return achieves.Any() ? Ok(achieves) : NotFound();
        }*/
    }
}