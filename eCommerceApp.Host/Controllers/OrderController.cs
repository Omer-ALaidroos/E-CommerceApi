using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Implementation.OrderServices.query;
using eCommerceApp.Domain.Interfaces.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService,IMediator _mediator) : ControllerBase
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

        [HttpPost("Create")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromBody] Checkout checkout)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            checkout.UserId = GetUserId();
            var response = await orderService.CreateOrder(checkout);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
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

            var result = await _mediator.Send(
                new GetUserOrdersQuery(userId!));

            return Ok(result);
        }

        [HttpGet("PendingOrders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _mediator.Send(
              new GetPendingOrdersQuery());

            return Ok(result);
        }
        [HttpGet("OrderSummaries")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrderSummaries()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _mediator.Send(
                new GetUserOrderSummariesQuery(userId));

            return Ok(result);
        }
        [HttpGet("GetById/{orderId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _mediator.Send(
                new GetUserOrderByIdQuery(orderId, userId));

            if (result is null)
                return NotFound();

            return Ok(result);
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await orderService.DeleteORderAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
