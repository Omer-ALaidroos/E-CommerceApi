using eCommerceApp.Application.DependencyInjection;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.RegisterPolicy)]
        public async Task<IActionResult> CreateUser(CreateUser user)
        {
            var result = await authenticationService.CreateUser(user);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.LoginPolicy)]
        public async Task<IActionResult> LoginUser(LoginUser user)
        {
            var result = await authenticationService.LoginUser(user);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /*[HttpGet("refreshtoken")]
        public async Task<IActionResult> ReviveToken(string refreshToken)
        {
            var result = await authenticationService.ReviveToken(refreshToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }*/
        [HttpGet("refreshtoken")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.LoginPolicy)]
        public async Task<IActionResult> ReviveToken([FromQuery] string refreshToken)
        {
            // 1. We use [HttpGet] with [FromQuery] to match the Flutter code: 
            // Dio().get(..., queryParameters: {"refreshToken": refreshToken})

            var result = await authenticationService.ReviveToken(refreshToken);

            if (result.Success)
            {
              
                return Ok(result);
            }

            return Unauthorized(new { message = result.Message });
        }
    
        [HttpPost("ChangePassword")]
        [Authorize(Roles = "User")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.PublicApiPolicy)]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ServicesResponse(Message: "User ID not found in claims."));
            var result = await authenticationService.ChangePassword(changePassword, userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("ForgotPassword")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.ForgotPasswordPolicy)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var result = await authenticationService.ForgotPassword(model);
            return Ok(result);
        }

        [HttpPost("VerifyOTPCode")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.OTPPolicy)]
        public async Task<IActionResult> VerifyOTPCode([FromBody] VerifyResetCodeDto model)
        {
            var result = await authenticationService.VerifyResetCode(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        [EnableRateLimiting(RateLimitingExtensions.RateLimitingPolicyNames.ForgotPasswordPolicy)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await authenticationService.ResetPassword(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}