using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Infrastructure.Data;
using eCommerceApp.Infrastructure.Middleware;
using eCommerceApp.Infrastructure.Repository;
using eCommerceApp.Infrastructure.Sevices;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Repository.Authentication;
using eCommerceApp.Domain.Interfaces.Cart;
using eCommerceApp.Infrastructure.Repository.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Interfaces.CategorySpecifics;
using eCommerceApp.Infrastructure.Repository.CategorySpecifics;

namespace eCommerceApp.Infrastructure.Dependency_Injection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Default"),
                SqlOption =>
                {
                    SqlOption.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    SqlOption.EnableRetryOnFailure();


                }).UseExceptionProcessor(),
                 ServiceLifetime.Scoped
                );


            services.AddScoped<IGeneric<Product>, GenericRepository<Product>>();
            services.AddScoped<IGeneric<Category>, GenericRepository<Category>>();
            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdapter<>));
            services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
            }).AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!)),

                    ValidAudience = config["JWT:Audience"],
                    ValidIssuer = config["JWT:Issuer"],


                };
            }

            );

            services.AddScoped<IUserManagement, UserManagement>();
            services.AddScoped<IRoleManagement, RoleManagement>();
            services.AddScoped<ITokenManagements, TokenManagement>();
            services.AddScoped<IPaymentMethod,PaymentMethodRepository>();
            services.AddScoped<IPaymentService, StripPaymentService>();
            services.AddScoped<ICategory, CategoryRepository>();
            services.AddScoped<ICart, CartRepository>();
          
            Stripe.StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            return services;

        }
        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
         

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}
