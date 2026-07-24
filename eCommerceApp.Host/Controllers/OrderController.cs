using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.DTOs.Payment;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Application.Services.Interfaces.Payment;
using eCommerceApp.Domain.Interfaces.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService, IMediator mediator, IPaymentService paymentService) : ControllerBase
    {
        

        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var Orders = await orderService.GetAllAsync();

            return Orders.Any() ? Ok(Orders) : NotFound();
        }


       /* [HttpGet("Single/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var Order = await orderService.GetByIdAsync(id);

            return Order != null ? Ok(Order) : NotFound();
        }*/

        [HttpPost("checkout")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Checkout([FromBody] Checkout checkout)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            checkout.UserId = GetUserId();
            var response = await orderService.CreateOrder(checkout);

            if (!response.IsSuccess)
                return BadRequest(response);

            var orderId = response.Data as int? ?? 0;
            if (orderId <= 0)
                return BadRequest(new { message = "Order id was not returned after creation." });

            var userId = GetUserId();
            var paymentRequest = new CreatePaymentIntentRequestDto
            {
                OrderId = orderId,
                Currency = "usd",
                Description = $"Order {orderId}"
            };

            var paymentResult = await paymentService.CreatePaymentIntentAsync(paymentRequest, userId);

            return paymentResult.IsSuccess
                ? Ok(new { order = response, payment = paymentResult.Data })
                : BadRequest(new { order = response, payment = paymentResult });
        }
        /* public async Task<IActionResult> Add([FromBody] CreateOrder Order)
         {

             if (!ModelState.IsValid)
                 return BadRequest(ModelState);

            // var response = await orderService.AddAsync(Order);
             return response.IsSuccess ? Ok(response) : BadRequest(response);
         }
        */
        [HttpGet("UserOrders")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await mediator.Send(
                new GetUserOrdersQuery(userId!));

            return Ok(result);
        }

        [HttpGet("OrdersByStatus/{Status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByStatus(string Status)
        {
            var result = await mediator.Send(
              new GetOrdersSummariesByStatusQuery(Status));

            return Ok(result);
        }

        [HttpPut("UpdateStatus/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto? request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request is null)
            {
                var response = await orderService.UpdateOrderStatusAsync(orderId);
                return response.IsSuccess ? Ok(response) : BadRequest(response);
            }

            request.Id = orderId;
            var explicitResponse = await orderService.UpdateOrderStatusAsync(request);
            return explicitResponse.IsSuccess ? Ok(explicitResponse) : BadRequest(explicitResponse);
        }
        [HttpGet("OrderSummaries")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrderSummaries()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await mediator.Send(
                new GetUserOrderSummariesQuery(userId));

            return Ok(result);
        }
        [HttpGet("GetById/{orderId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetById(int orderId)
        {
          

            var result = await mediator.Send(
                new GetUserOrderByIdQuery(orderId));

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("GetStatusByOrderId/{orderId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetStatusByOrderId(int orderId)
        {
            var response = await orderService.GetOrderStatusByIdAsync(orderId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await orderService.DeleteOrderAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Cancel/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await orderService.CancelOrderAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
