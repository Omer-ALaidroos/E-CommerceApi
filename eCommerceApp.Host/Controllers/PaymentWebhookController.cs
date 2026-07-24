using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Interfaces.Payment;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentWebhookController(IStripeWebhookService stripeWebhookService) : ControllerBase
    {
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(
        [FromHeader(Name = "Stripe-Signature")] string stripeSignature)
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(stripeSignature) ||
                string.IsNullOrWhiteSpace(payload))
            {
                return BadRequest(new { message = "Invalid webhook payload." });
            }

            var result = await stripeWebhookService.HandleWebhookAsync(
                payload,
                stripeSignature);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Webhook Error: {result.Message}");
                return BadRequest(result);
            }

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
