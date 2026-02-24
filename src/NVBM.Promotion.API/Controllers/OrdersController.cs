using Microsoft.AspNetCore.Mvc;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Sales.Queries;
using Wolverine;

namespace NVBM.Promotion.API.Controllers;

[ApiController]
[Route("api/v1/Orders")]
public class OrdersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public OrdersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateOrder([FromBody] OrderCalculateRequest request)
    {
        var response = await _messageBus.InvokeAsync<APIResponse<OrderCalculateResponse>>(new CalculateOrderQuery(request));
        
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
