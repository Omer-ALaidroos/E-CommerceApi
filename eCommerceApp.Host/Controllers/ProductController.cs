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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var Products = await productServise.GetAllAsync();

           return  Products.Any() ? Ok(Products) : NotFound();
        }
        [HttpGet("Available")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var Products = await productServise.GetAvailableProductsAsync();

            return Products.Any() ? Ok(Products) : NotFound();
        }

        [HttpGet("GetAvaliableByCategoryId")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAvaliableProductsByCategoryId(int categoryId)
        {
            var Products = await productServise.GetAvaliableProductsByCategoryId(categoryId);

            return Products.Any() ? Ok(Products) : NotFound();
        }


        [HttpGet("Single/{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var Product = await productServise.GetByIdAsync(id);

            return Product != null ? Ok(Product) : NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] CreateProduct product,IFormFile image)

        {
            Console.WriteLine(image == null ? "No image uploaded" : $"Image uploaded: {image.FileName}");
            Console.WriteLine($"Received product: {product.Name}, Image: {image?.FileName}");   
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await productServise.AddAsync(product,image);

            Console.WriteLine($"Service response: Success={response.IsSuccess}, Message={response.Message}");
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateProduct product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var response = await productServise.UpdateAsync(product, product.Image);

            return response.IsSuccess
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await productServise.DeleteAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetProductByCategoryId")]
        [Authorize(Roles ="Admin,User")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            var Products = await productServise.GetProductsByCategoryAsync(categoryId);

            return Products.Any() ? Ok(Products) : NotFound();
        }
    }
}
