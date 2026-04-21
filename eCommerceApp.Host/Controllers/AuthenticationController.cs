using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {

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

    }
}