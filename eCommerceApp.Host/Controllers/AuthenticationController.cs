using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUser user)
        {
            var result = await authenticationService.CreateUser(user);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginUser user)
        {
            var result = await authenticationService.LoginUser(user);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("refreshtoken")]
        public async Task<IActionResult> ReviveToken([FromBody]string refreshToken)
        {
            var result = await authenticationService.ReviveToken(refreshToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

            [HttpPost("ChangePassword")]
            [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePassword changePassword)
            {
                
                string userId =GetUserId();
            if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ServicesResponse(Message: "User ID not found in claims."));
                var result = await authenticationService.ChangePassword(changePassword, userId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var result = await authenticationService.ForgotPassword(model);
            return Ok(result); // Always 200 for security
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDto model)
        {
            var result = await authenticationService.VerifyResetCode(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await authenticationService.ResetPassword(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}