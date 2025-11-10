using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productServise) : ControllerBase
    {

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var Products = await productServise.GetAllAsync();

           return  Products.Any() ? Ok(Products) : NotFound();
        }


        [HttpGet("Single/{id}")]
        public async Task<IActionResult> GetSingle(Guid id)
        {
            var Product = await productServise.GetByIdAsync(id);

            return Product != null ? Ok(Product) : NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CreateProduct product)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await productServise.AddAsync(product);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProduct product)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await productServise.UpdateAsync(product);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await productServise.DeleteAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
