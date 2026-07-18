using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementation
{
    public class CategoryService(ICategory CategoryInterface,
     IMapper mapper) : ICategoryService
    {
        public async Task<ServicesResponse> AddAsync(CreateCategory category)
        {
            var mappedCategory = mapper.Map<Category>(category);

            int result = await CategoryInterface.AddAsync(mappedCategory);

            if (result > 0)
            {
                return new ServicesResponse(true, "Category added successfully.");

            }
            else
            {
                return new ServicesResponse(false, "Failed to add Category.");

            }
        }

        public async Task<ServicesResponse> DeleteAsync(int id)
        {
            int result = await CategoryInterface.DeleteAsync(id);

            if (result == 0)
                return new ServicesResponse(false, "Failed to delete Category.");


            return result > 0 ?
                new ServicesResponse(true, "Category delete successfully.") :
                new ServicesResponse(false, "Failed to delete Category."); ;

        }

        public async Task<IEnumerable<GetCategory>> GetAllAsync()
        {
            var Categories = await CategoryInterface.GetAllAsync();

            if (!Categories.Any()) return [];

            return mapper.Map<IEnumerable<GetCategory>>(Categories);
        }

        public async Task<GetCategory> GetByIdAsync(int id)
        {
            var Category = await CategoryInterface.GetByIdAsync(id);
            return mapper.Map<GetCategory>(Category);
        }

      

        public async Task<ServicesResponse> UpdateAsync(UpdateCategory category)
        {
            var mappedCategory = mapper.Map<Category>(category);
            int result = await CategoryInterface.UpdateAsync(mappedCategory);
            if (result > 0)
            {
                return new ServicesResponse(true, "Category updated successfully.");
            }
            else
            {
                return new ServicesResponse(false, "Failed to update Category.");
            }
        }
    }
}
