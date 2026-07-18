using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController(IAddressService addressService) : ControllerBase
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
            var Address = await addressService.GetAllAsync();

            return Address.Any() ? Ok(Address) : NotFound();
        }


        [HttpGet("Single/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var Address = await addressService.GetByIdAsync(id);

            return Address != null ? Ok(Address) : NotFound();
        }

        [HttpGet("GetUserAddress")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserAddress()
        {
            string userId = GetUserId();
            var Address = await addressService.GetUserAddressAsync(userId);

            return Address != null ? Ok(Address) : NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Add([FromBody] CreateAddress Address)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Address.UserId = GetUserId();

            var response = await addressService.AddAsync(Address);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update([FromBody] UpdateAddress Address)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await addressService.UpdateAsync(Address);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("Delete/{idAddress}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int idAddress)
        {
            var response = await addressService.DeleteAsync(idAddress);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

    }
}
