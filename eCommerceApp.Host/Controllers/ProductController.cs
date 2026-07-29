﻿using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        private string GetUserId()
        {
            return User.FindFirst("uid")?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("available")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAvailable()
        {
            var userId = GetUserId();
            var products = await _productService.GetAvailableProductsAsync(userId);
            return Ok(products);
        }

        [HttpGet("GetByIdForUser/{id}")]
        [Authorize(Roles = "User,Admin")]
       
        public async Task<IActionResult> GetByIdForUser(int id)
        {
            var product = await _productService.GetByIdForUserAsync(id);
            if (product.Id == 0)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("GetByIdForAdmin/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var product = await _productService.GetByIdForAdminAsync(id);
            if (product.Id == 0)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
           var userId = GetUserId();
            var products = await _productService.GetAvaliableProductsByCategoryId(userId, categoryId);
            return Ok(products);
        }

        [HttpGet("search")]
        [Authorize(Roles = "User,Admin")]
        
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty.");
            }
            var products = await _productService.SearchByNameAsync(name);
            return Ok(products);
        }
        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] CreateProduct product)
        {
            if (product.Images == null || product.Images.Count == 0)
            {
                return BadRequest("At least one image is required.");
            }

            var result = await _productService.AddAsync(product, product.Images);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] UpdateProduct product, [FromForm]List<IFormFile>? images)
        {
            var result = await _productService.UpdateAsync(product, images);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("AddToFavorite/{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddToFavorite(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _productService.AddToFavoriteAsync(userId, id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("RemoveFromFavorite/{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> RemoveFromFavorite(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _productService.RemoveFromFavoriteAsync(userId, id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("MyFavorites")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var products = await _productService.GetFavoriteProductsByUserAsync(userId);
            return Ok(products);
        }
    }
}