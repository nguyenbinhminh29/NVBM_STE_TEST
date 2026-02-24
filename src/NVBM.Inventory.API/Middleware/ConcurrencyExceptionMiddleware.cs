using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using System.Net;
using System.Text.Json;

namespace NVBM.Inventory.API.Middleware;

public class ConcurrencyExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ConcurrencyExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateConcurrencyException)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            var response = APIResponse<bool>.Fail("Sản phẩm đã thay đổi trạng thái, vui lòng tải lại.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
