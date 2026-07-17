using System.Security.Claims;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController(IProductReviewService reviewService) : ControllerBase
    {
        private string GetUserId()
            => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

        [HttpPost("Add")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddReview([FromBody] CreateProductReviewDto dto)
        {
            var userId = GetUserId();
            var result = await reviewService.AddReviewAsync(userId, dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("can-review/{productId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CanReview(int productId)
        {
            var userId = GetUserId();
            var result = await reviewService.CanUserReviewProductAsync(userId, productId);
            return  Ok(result);
        }

        [HttpPost("hide/{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideReview(string reviewId)
        {
            var result = await reviewService.HideReviewAsync(reviewId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("show/{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowReview(string reviewId)
        {
            var result = await reviewService.ShowReviewAsync(reviewId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
