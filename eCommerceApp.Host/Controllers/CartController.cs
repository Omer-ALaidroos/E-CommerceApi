using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("checkout")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Checkout([FromBody] Checkout checkout)
        {
           if(!ModelState.IsValid)
               return BadRequest(ModelState);

           var result = await cartService.Checkout(checkout);
           return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Save-Checkout")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> SaveCheckoutHistory([FromBody] IEnumerable<CreateAchieve> achieves)
        {


            var result = await cartService.SaveCheckoutHistory(achieves);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-achieves")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllCheckoutHistory()
        {
            var achieves = await cartService.GetAchieves();
            return achieves.Any() ? Ok(achieves) : NotFound();
        }

    }
}
