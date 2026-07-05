using AutoMapper;
using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Entities;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Address;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Entities.Orders;

namespace eCommerceApp.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreateCategory, Category>();
            CreateMap<CreateProduct, Product>();

            CreateMap<Category, GetCategory>();
            CreateMap<Product, GetProduct>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Images.FirstOrDefault(i => i.IsPrimary) != null
                        ? src.Images.FirstOrDefault(i => i.IsPrimary).ImageUrl
                        : null));

            CreateMap<CreateUser,AppUser>();
            CreateMap<LoginUser,AppUser>();

            CreateMap<PaymentMethod, GetPaymntMethod>();
            CreateMap<UpdateCategory, Category>();
            CreateMap<UpdateProduct, Product>();

            CreateMap<CreateAddress, Address>();
            CreateMap<UpdateAddress, Address>();
            CreateMap<Address, GetAddress>();

            CreateMap<Order, GetOrder>();
            CreateMap<CreateOrder,Order>();

            CreateMap<AppUser, GetUser>();
        }

       
    }
}
