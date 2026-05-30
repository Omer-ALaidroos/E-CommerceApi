using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {

        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        private string GetUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value!;
        }

        [HttpPut("UpdateFullName")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateFullName( string fullName)
        {
            var userId = GetUserId();
            var response = await userService.EditFullName(fullName, userId);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpPut("UpdatePhoneNumber")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdatePhoneNumber( string phoneNumber)
        {
            var userId = GetUserId();
            var response = await userService.EditPhoneNumber(phoneNumber, userId);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpGet("GetUserById")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserById()
        {
            var userId = GetUserId();
             var User = await userService.GetByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            return Ok(User);
        }

        [HttpGet("GetUserByEmail")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByEmail()
        {
            var email = GetUserEmail();
            var user = await userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        }
}
