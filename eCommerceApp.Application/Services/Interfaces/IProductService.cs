﻿using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using Microsoft.AspNetCore.Http;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();
        Task<IEnumerable<GetProduct>> GetAvailableProductsAsync();
        Task<IEnumerable<GetProduct>> GetAvaliableProductsByCategoryId(int categoryID);

        Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(int categoryId);
        Task<GetProduct> GetByIdAsync(int id);
        Task<ServicesResponse> AddAsync(CreateProduct product, IFormFileCollection images);
        Task<ServicesResponse> UpdateAsync(UpdateProduct product, IFormFileCollection? images);
        Task<ServicesResponse> DeleteAsync(int id);
        Task<IEnumerable<GetProduct>> SearchByNameAsync(string name);
    }

    
}
