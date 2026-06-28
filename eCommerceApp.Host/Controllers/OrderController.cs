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

        [HttpGet("OrdersByStatus/{Status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByStatus(string Status)
        {
            var result = await _mediator.Send(
              new GetOrdersSummariesByStatusQuery(Status));

            return Ok(result);
        }

        [HttpPut("UpdateStatus/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await orderService.UpdateOrderStatusAsync(orderId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
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
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetById(int orderId)
        {
          

            var result = await _mediator.Send(
                new GetUserOrderByIdQuery(orderId));

            if (result is null)
                return NotFound();

            return Ok(result);
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
