using Wolverine;
using Wolverine.FluentValidation;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using NVBM.Infrastructure.Data;
using Scalar.AspNetCore;
using NVBM.Inventory.API.Middleware;

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

// Distributed Cache for Idempotency
builder.AddRedisDistributedCache("cache");

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


// Wolverine
builder.Host.UseWolverine(opts => 
{
    opts.UseFluentValidation();
    opts.Discovery.IncludeAssembly(typeof(NVBM.Application.Features.Inventory.Commands.UpdateInventoryCommand).Assembly);
    opts.Discovery.IncludeAssembly(typeof(NVBM.Infrastructure.Features.Inventory.Handlers.UpdateInventoryCommandHandler).Assembly);
});

builder.Services.AddScoped<NVBM.Application.Interfaces.IInventoryRepository, NVBM.Infrastructure.Repositories.InventoryRepository>();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "NVBM Inventory API";
        document.Info.Version = "v1";
        document.Info.Description = "API for handling high-throughput Inventory operations.";
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
app.UseMiddleware<ConcurrencyExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();
