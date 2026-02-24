using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Services;
using Scalar.AspNetCore;
using NVBM.Catalog.API.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using NVBM.Application.DTOs;
using Wolverine;
using Wolverine.FluentValidation;
using Serilog;
using NVBM.Catalog.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.UseSerilog((context, loggerConfig) => 
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
    loggerConfig.WriteTo.Console();
});

// Aspire Service Defaults
builder.AddServiceDefaults();

// Database
builder.AddSqlServerDbContext<NVBMDbContext>("grocerydb");

// Cache
builder.AddRedisOutputCache("cache");

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

// Add services to the container.
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProductRepository, NVBM.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<IPromotionRepository, NVBM.Infrastructure.Repositories.PromotionRepository>();
builder.Services.AddScoped<IPromotionEngine, NVBM.Infrastructure.Services.PromotionEngine>();
builder.Services.AddScoped<IEventPublisher, WolverineEventPublisher>();
builder.Services.AddScoped<IProductCatalogService, ProductCatalogService>();

// Wolverine
builder.Host.UseWolverine(opts => 
{
    opts.UseFluentValidation();
    // Configure rabbitmq or endpoints later
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "NVBM Product Catalog API";
        document.Info.Version = "v1";
        document.Info.Description = "API for Product Catalog Management.";
        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseOutputCache();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

await app.ApplyMigrationsAndSeedAsync();

app.Run();
