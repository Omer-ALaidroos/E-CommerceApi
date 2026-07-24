using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Interfaces.CartInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PaymentServiceInterface = eCommerceApp.Application.Services.Interfaces.Payment.IPaymentService;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController(IPaymentMethodService paymentMethodeService, PaymentServiceInterface paymentService) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpGet("Methods")]
        public async Task<ActionResult<IEnumerable<GetPaymntMethod>>> GetPaymentMethods()
        {
            var paymentMethods = await paymentMethodeService.GetPaymntMethods();
            if (!paymentMethods.Any()) return NotFound();

            return Ok(paymentMethods);
        }

        /*[HttpPost("create-intent")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentApiRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "User ID not found in claims." });

            var applicationRequest = new CreatePaymentIntentRequestDto
            {
                OrderId = request.OrderId,
                Amount = 0.01m,
                Currency = "usd",
                Description = $"Order {request.OrderId}"
            };

            var result = await paymentService.CreatePaymentIntentAsync(applicationRequest, userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
*/
        public class CreatePaymentIntentApiRequest
        {
            public int OrderId { get; set; }
        }
    }
}
