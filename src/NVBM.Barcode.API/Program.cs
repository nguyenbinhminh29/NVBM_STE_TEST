using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire Service Defaults
builder.AddServiceDefaults();

// Database
builder.AddSqlServerDbContext<NVBMDbContext>("grocerydb");

// Cache
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddScoped<IBarcodeService, BarcodeService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "NVBM Barcode Management API";
        document.Info.Version = "v1";
        document.Info.Description = "API for Barcode Lookup Management.";
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
app.UseAuthorization();
app.MapControllers();

app.Run();
