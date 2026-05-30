using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IEmailService emailService) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendTestEmail([FromBody] string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return BadRequest("Email address is required.");

            var subject = "Test Email from eCommerceApp";
            var body = "<h1>Success!</h1><p>This is a test email sent using <strong>Brevo SMTP</strong> and MailKit.</p>";

            await emailService.SendEmailAsync(emailAddress, subject, body);

            return Ok(new { Message = $"Test email sent to {emailAddress}" });
        }
    }
}