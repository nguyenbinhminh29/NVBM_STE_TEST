using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Services;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.FluentValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using NVBM.Promotion.API.Middleware;
using NVBM.Promotion.API.Extensions;

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
builder.Services.AddMemoryCache();
builder.AddRedisOutputCache("cache");

// Services Register
builder.Services.AddScoped<IProductRepository, NVBM.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<IPromotionRepository, NVBM.Infrastructure.Repositories.PromotionRepository>();
builder.Services.AddScoped<IPromotionEngine, PromotionEngine>();

// Wolverine
builder.Host.UseWolverine(opts => 
{
    opts.UseFluentValidation();
    opts.Discovery.IncludeAssembly(typeof(NVBM.Application.Features.Sales.Queries.CalculateOrderQuery).Assembly);
    opts.Discovery.IncludeAssembly(typeof(NVBM.Infrastructure.Features.Sales.Handlers.CalculateOrderQueryHandler).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "NVBM Promotion API";
        document.Info.Version = "v1";
        document.Info.Description = "API for Promotion Rules Engine and Order Calculation.";
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
app.UseAuthorization();
app.MapControllers();

await app.SeedPromotionDataAsync();

app.Run();
