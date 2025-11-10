using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eCommerceApp.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
           try
           {
                await _next(context);
           }
           catch (DbUpdateException ex)
           {
                var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
                context.Response.ContentType = "application/json";
                if (ex.InnerException is SqlException InnerExcepion)
                {
                    logger.LogError(InnerExcepion, "sql exception.");
                    switch (InnerExcepion.Number)
                    {

                        case 2627: // Unique constraint error
                        case 2601: // Duplicated key row error
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            await context.Response.WriteAsync("A conflict occurred due to duplicate data.");
                            break;
                        case 515:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsync("Can not insert null");
                            break;
                        case 547: // Constraint check violation
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            await context.Response.WriteAsync("Foreign key constraint violation.");
                            break;
                        default:

                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            await context.Response.WriteAsync("An  error occurred while saving entity change.");
                            break;
                    }
                }
                else
                {
                    logger.LogError(ex, "related EFcore Exception");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("An  error occurred while saving entity change.");
                }
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
                logger.LogError(ex, "An UnKown exception occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An internal server error occurred ." + ex.Message);
            }
        }
    }
}
