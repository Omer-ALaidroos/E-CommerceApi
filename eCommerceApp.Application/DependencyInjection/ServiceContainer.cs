using eCommerceApp.Application.Mapping;
using eCommerceApp.Application.Services.Implementation;
using eCommerceApp.Application.Services.Implementation.Cart;
using eCommerceApp.Application.Services.Implementation.OrderServices;
using eCommerceApp.Application.Services.Implementation.OrderServices.command;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.CartInterface;
using eCommerceApp.Application.Validations.Authentication;
using eCommerceApp.Domain.Interfaces.Orders;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerceApp.Application.DependencyInjection
{
    public static class ServiceContainer
    {
       
        public static IServiceCollection AddApplicationServices
            (this IServiceCollection services)
        {
            // register AutoMapper profiles from this assembly
            services.AddAutoMapper(typeof(MappingConfig).Assembly);
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddScoped<IValidationsService, ValidationsService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ImageUploader, ImageUploader>();



            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUserOrdersQueryHandler>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUserOrderByIdQueryHandler>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUserOrderSummariesQueryHandler>());
        

            return services;
        }
    }
}
