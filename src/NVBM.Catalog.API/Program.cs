using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Services;
using Scalar.AspNetCore;
using NVBM.Catalog.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IProductCatalogService, ProductCatalogService>();

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

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

await app.ApplyMigrationsAndSeedAsync();

app.Run();
