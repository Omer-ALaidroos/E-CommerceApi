using eCommerceApp.Application.DependencyInjection;
using eCommerceApp.Application.Mapping;
using eCommerceApp.Infrastructure.Dependency_Injection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Services.AddAutoMapper(typeof(MappingConfig).Assembly);



builder.Host.UseSerilog();
Log.Logger.Information("Application is Building ...");
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

builder.Services.AddCors(builder =>
{
    builder.AddDefaultPolicy(options =>
    {
        options.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:5214")
              .AllowCredentials();
    });
});



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerceApp API", Version = "v1" });

    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

  
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});


try
{
    var app = builder.Build();
    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        // Show startup exceptions in the browser so we can see what's breaking Swagger generation
        app.UseDeveloperExceptionPage();

        // Serve Swagger early so it's not blocked by other middleware
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            // explicit endpoint so UI loads the correct document
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerceApp API v1");
            // If you want the UI at the app root, uncomment:
            // c.RoutePrefix = string.Empty;
        });
    }

    app.UseInfrastructureService();
    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Logger.Information("Applicationis running ..");
    app.Run();

}catch(Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}