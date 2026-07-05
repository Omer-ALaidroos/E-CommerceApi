using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace eCommerceApp.Application.Services.Implementation
{
    public class ProductService(IProduct ProductInterface, IMapper mapper,ImageUploader imageUploader) : IProductService
    {
        public async Task<ServicesResponse> AddAsync(CreateProduct product, IFormFileCollection images)
        {
            if (images == null || images.Count == 0)
            {
                return new ServicesResponse(false, "Image file is required.");
            }

            var mappedProduct = mapper.Map<Product>(product);

            // Ensure Images collection exists
            if (mappedProduct.Images == null)
                mappedProduct.Images = new List<ProductImage>();

            int order = 1;
            bool first = true;
            foreach (var file in images)
            {
                var imagePath = await imageUploader.UploadImage(file);
                if (imagePath == null)
                {
                    return new ServicesResponse(false, "Image not saved ,please upload image with extention jpg,jpeg,png");
                }

                var img = new ProductImage
                {
                    ImageUrl = imagePath,
                    IsPrimary = first,
                    DisplayOrder = (short)order
                };

                mappedProduct.Images.Add(img);

                first = false;
                order++;
            }

            await ProductInterface.AddAsync(mappedProduct);
            int result = await ProductInterface.SaveChangesAsync();

            return result > 0
                ? new ServicesResponse(true, "Product added successfully.")
                : new ServicesResponse(false, "Failed to add product.");
        }

        public async Task<ServicesResponse> DeleteAsync(int id)
        {
            await ProductInterface.DeleteAsync(id);
            int result = await ProductInterface.SaveChangesAsync();

            return result > 0 ?
                new ServicesResponse(true, "Product delete successfully."):
                new ServicesResponse(false, "Failed to delete product."); ;

           
        }

        public async Task<IEnumerable<GetProduct>> GetAllAsync()
        {
           var products = await ProductInterface.GetAllAsync();

            if (!products.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(products);
        }

        public async Task<GetProduct> GetByIdAsync(int id)
        {
          var product =  await ProductInterface.GetByIdAsync(id);

          if(product == null) return new GetProduct();

           return mapper.Map<GetProduct>(product);
        }

        public async Task<IEnumerable<GetProduct>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await ProductInterface.GetProductsByCategory(categoryId);
            if (!products.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(products);
        }

        public async Task<ServicesResponse> UpdateAsync(UpdateProduct product, IFormFileCollection? images)
        {
            var existingProduct = await ProductInterface.GetByIdAsync(product.Id);

            if (existingProduct == null)
            {
                return new ServicesResponse(false, "Product not found");
            }

            mapper.Map(product, existingProduct);

            if (images != null && images.Count > 0)
            {
                // Handle uploaded images: set first uploaded as new primary (replace old primary), add others
                var oldPrimary = existingProduct.Images.FirstOrDefault(pi => pi.IsPrimary);

                bool firstNew = true;
                int nextOrder = existingProduct.Images.Any() ? existingProduct.Images.Max(i => i.DisplayOrder) + 1 : 1;

                foreach (var file in images)
                {
                    var imagePath = await imageUploader.UploadImage(file);
                    if (imagePath == null)
                    {
                        return new ServicesResponse(false, "Image not saved, please upload jpg, jpeg or png");
                    }

                    var newImage = new ProductImage
                    {
                        ImageUrl = imagePath,
                        IsPrimary = false,
                        DisplayOrder = (short)nextOrder
                    };

                    existingProduct.Images.Add(newImage);

                    if (firstNew)
                    {
                        // Replace primary
                        if (oldPrimary != null)
                        {
                            // delete old primary file
                            await imageUploader.DeleteImage(oldPrimary.ImageUrl);
                            oldPrimary.IsPrimary = false;
                        }

                        newImage.IsPrimary = true;
                        firstNew = false;
                    }

                    nextOrder++;
                }
            }

            int result = await ProductInterface.SaveChangesAsync();

            return result > 0
                ? new ServicesResponse(true, "Product updated successfully.")
                : new ServicesResponse(false, "Failed to update product.");
        }
        public async Task<bool> DecreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await ProductInterface.GetByIdAsync(productId);
            if (product == null)
            {
                return false; // Product not found
            }

            if (product.Quantity < quantity)
            {
                return false; // Insufficient stock
            }

            product.Quantity -= quantity;
            // UpdateAsync no longer saves changes, so we need to save here
            await ProductInterface.UpdateAsync(product);
            int result = await ProductInterface.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> IncreaseProductQuantityAsync(int productId, int quantity)
        {
            var product = await ProductInterface.GetByIdAsync(productId);
            if (product == null) return false; // Product not found
            product.Quantity += quantity;
            // UpdateAsync no longer saves changes, so we need to save here
            await ProductInterface.UpdateAsync(product);
            int result = await ProductInterface.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<GetProduct>> GetAvailableProductsAsync()
        {
            var products = await ProductInterface.GetAvailableProductsAsync();

            if (!products.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(products);
        }

        public async Task<IEnumerable<GetProduct>> GetAvaliableProductsByCategoryId(int categoryID)
        {
            var products = await ProductInterface.GetAvailableProductsByCategoryAsync(categoryID);
            if (!products.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(products);
        }

        public async Task<IEnumerable<GetProduct>> SearchByNameAsync(string name)
        {
            var products = await ProductInterface.SearchByNameAsync(name);
            if (!products.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(products);
        }
    }
}
