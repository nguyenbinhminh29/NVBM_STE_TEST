using System.Net;
using FluentValidation;
using NVBM.Application.DTOs;
using Microsoft.AspNetCore.Diagnostics;

namespace NVBM.Shift.API.Middleware;

public static class ExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.ContentType = "application/json";
                    
                    if (contextFeature.Error is ValidationException validationException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
                        
                        var responseMessage = "Validation failed: " + string.Join("; ", errors);
                        var response = APIResponse<object>.Fail(responseMessage);
                        
                        await context.Response.WriteAsJsonAsync(response);
                        return;
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var internalError = APIResponse<object>.Fail("An internal server error occurred in Shift API.");
                    await context.Response.WriteAsJsonAsync(internalError);
                }
            });
        });
    }
}
