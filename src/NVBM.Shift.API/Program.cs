using FluentValidation;
using NVBM.Application.Features.Shift.Commands;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Repositories;
using NVBM.Shift.API.Middleware;
using Wolverine;
using Wolverine.FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<NVBMDbContext>("grocerydb");
builder.AddRedisOutputCache("cache");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Dependency Injection
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();

// Wolverine & FluentValidation
builder.Host.UseWolverine(opts =>
{
    opts.UseFluentValidation();
    opts.Discovery.IncludeAssembly(typeof(OpenShiftCommand).Assembly);
    opts.Discovery.IncludeAssembly(typeof(NVBM.Infrastructure.Features.Shift.Handlers.ShiftCommandHandlers).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<OpenShiftCommandValidator>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("NVBM Shift Management API");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseGlobalExceptionHandler(); // Use custom exception handler

app.MapControllers();

app.Run();
