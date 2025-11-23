﻿using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet("All")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var Categories = await categoryService.GetAllAsync();

            return Categories.Any() ? Ok(Categories) : NotFound();
        }


        [HttpGet("Single/{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var category = await categoryService.GetByIdAsync(id);

            return category != null ? Ok(category) : NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CreateCategory category)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await categoryService.AddAsync(category);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateCategory category)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await categoryService.UpdateAsync(category);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await categoryService.DeleteAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("Products-by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await categoryService.GetProductsByCategoryAsync(categoryId);
            return products.Any() ? Ok(products) : NotFound();
        }
    }
}
