using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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
    
    // Default Scalar UI for Gateway itself (if any endpoints)
    app.MapScalarApiReference();
    
    // Scalar UI for Catalog API: /scalar/catalog
    app.MapScalarApiReference("scalar/catalog", options =>
    {
        options.Title = "NVBM Product Catalog API";
        options.OpenApiRoutePattern = "/openapi/catalog.json";
    });

    // Scalar UI for Barcode API: /scalar/barcode
    app.MapScalarApiReference("scalar/barcode", options =>
    {
        options.Title = "NVBM Barcode Management API";
        options.OpenApiRoutePattern = "/openapi/barcode.json";
    });
}

app.MapReverseProxy();

app.Run();
