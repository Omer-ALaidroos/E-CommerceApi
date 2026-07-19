using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace eCommerceApp.Application.DependencyInjection
{
    public static class RateLimitingExtensions
    {
        public static class RateLimitingPolicyNames
        {
            public const string PublicApiPolicy = "public";
            public const string RegisterPolicy = "register";
            public const string LoginPolicy = "login";
            public const string ForgotPasswordPolicy = "forgot-password";
            public const string OTPPolicy = "otp";
        }

        public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(RateLimitingPolicyNames.PublicApiPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                options.AddPolicy(RateLimitingPolicyNames.RegisterPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                options.AddPolicy(RateLimitingPolicyNames.LoginPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                options.AddPolicy(RateLimitingPolicyNames.ForgotPasswordPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(5)
                        }));

                options.AddPolicy(RateLimitingPolicyNames.OTPPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1)
                        }));
            });

            return services;
        }
    }
}