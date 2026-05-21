using eCommerceApp.Application.DTOs.Cart;

using eCommerceApp.Application.Services.Interfaces.CartInterface;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPaymentMethodService paymentMethodeService) : ControllerBase
    {

        [HttpGet("Methods")]
        public async Task<ActionResult<IEnumerable<GetPaymntMethod>>> GetPaymentMethods()
        {
            var paymentMethods = await paymentMethodeService.GetPaymntMethods();
           if (!paymentMethods.Any()) return NotFound();

            return Ok(paymentMethods);
        }
    }
}
