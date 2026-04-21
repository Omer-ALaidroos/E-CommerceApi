using ECommerce.Core.DTOs.Order;
using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.Services.Implementation;
using eCommerceApp.Domain.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var Orders = await orderService.GetAllAsync();

            return Orders.Any() ? Ok(Orders) : NotFound();
        }


        [HttpGet("Single/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var Order = await orderService.GetByIdAsync(id);

            return Order != null ? Ok(Order) : NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "User")]
       /* public async Task<IActionResult> Add([FromBody] CreateOrder Order)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           // var response = await orderService.AddAsync(Order);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
       */
       

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await orderService.DeleteORderAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
