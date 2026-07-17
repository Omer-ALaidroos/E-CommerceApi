﻿using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using Microsoft.AspNetCore.Http;

namespace eCommerceApp.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();
        Task<IEnumerable<GetProduct>> GetAvailableProductsAsync(string userId);
        Task<IEnumerable<GetProduct>> GetAvaliableProductsByCategoryId(string userId, int categoryID);

        Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(int categoryId);
        Task<GetProductDetailsDto> GetByIdForUserAsync(int id);
        Task<GetProductDetailsDto> GetByIdForAdminAsync(int id);
        Task<ServicesResponse> AddAsync(CreateProduct product, List<IFormFile>? images);
        Task<ServicesResponse> UpdateAsync(UpdateProduct product, List<IFormFile>? images);
        Task<ServicesResponse> DeleteAsync(int id);
        Task<ServicesResponse> AddToFavoriteAsync(string userId, int productId);
        Task<ServicesResponse> RemoveFromFavoriteAsync(string userId, int productId);
        Task<IEnumerable<GetProduct>> GetFavoriteProductsByUserAsync(string userId);
        Task<IEnumerable<GetProduct>> SearchByNameAsync(string name);
    }

    
}
