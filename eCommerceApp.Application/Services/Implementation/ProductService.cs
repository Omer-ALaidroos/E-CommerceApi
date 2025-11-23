using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementation
{
    public class ProductService(IGeneric<Product> ProductInterface, IMapper mapper) : IProductService
    {
        public async Task<ServicesResponse> AddAsync(CreateProduct product)
        {
            var mappedProduct = mapper.Map<Product>(product);

            int result = await ProductInterface.AddAsync(mappedProduct);

            if (result > 0)
            {
                return new ServicesResponse(true, "Product added successfully.");

            }
            else
            {
                return new ServicesResponse(false, "Failed to add product.");

            }
        }

        public async Task<ServicesResponse> DeleteAsync(int id)
        {
            int result = await ProductInterface.DeleteAsync(id);

           

            
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

        public async Task<ServicesResponse> UpdateAsync(UpdateProduct product)
        {
            var mappedProduct = mapper.Map<Product>(product);
            int result = await ProductInterface.UpdateAsync(mappedProduct);
            if (result > 0)
            {
                return new ServicesResponse(true, "Product updated successfully.");
            }
            else
            {
                return new ServicesResponse(false, "Failed to update product.");
            }
        }
    }
}
