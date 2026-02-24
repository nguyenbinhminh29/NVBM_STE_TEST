using MediatR;
using Microsoft.AspNetCore.Mvc;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Inventory.Commands;

namespace NVBM.Inventory.API.Controllers;

[ApiController]
[Route("api/v1/Products/{id:guid}/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<ActionResult<APIResponse<bool>>> UpdateInventory(
        Guid id, 
        [FromBody] UpdateInventoryRequest dto,
        [FromHeader(Name = "RowVersion")] string rowVersionBase64)
    {
        if (string.IsNullOrEmpty(rowVersionBase64))
        {
            return BadRequest(APIResponse<bool>.Fail("Header RowVersion is required."));
        }

        var rowVersion = Convert.FromBase64String(rowVersionBase64);
        var command = new UpdateInventoryCommand(id, dto.Delta, rowVersion);
        
        var result = await _mediator.Send(command);
        
        if (!result.Success) return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult<APIResponse<bool>>> ReserveInventory(
        Guid id, 
        [FromBody] ReserveInventoryRequest dto,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            return BadRequest(APIResponse<bool>.Fail("Header Idempotency-Key is required."));
        }

        var command = new ReserveInventoryCommand(id, dto.Quantity, idempotencyKey);
        
        var result = await _mediator.Send(command);
        
        if (!result.Success) return BadRequest(result);
        
        return Ok(result);
    }
}

public record UpdateInventoryRequest(decimal Delta);
public record ReserveInventoryRequest(decimal Quantity);
