using Microsoft.AspNetCore.Mvc;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Shift.Commands;
using Wolverine;

namespace NVBM.Shift.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public ShiftsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenShift([FromBody] OpenShiftRequest request, CancellationToken cancellationToken)
    {
        var command = new OpenShiftCommand(request);
        var response = await _messageBus.InvokeAsync<APIResponse<ShiftResponseDto>>(command, cancellationToken);
        
        if (response.Success)
            return Ok(response);
            
        return BadRequest(response);
    }

    [HttpPost("{shiftId}/transactions")]
    public async Task<IActionResult> AddTransaction(Guid shiftId, [FromBody] ShiftTransactionRequest request, CancellationToken cancellationToken)
    {
        var command = new AddShiftTransactionCommand(shiftId, request);
        var response = await _messageBus.InvokeAsync<APIResponse<bool>>(command, cancellationToken);
        
        if (response.Success)
            return Ok(response);
            
        return BadRequest(response);
    }

    [HttpPost("{shiftId}/close")]
    public async Task<IActionResult> CloseShift(Guid shiftId, [FromBody] CloseShiftRequest request, CancellationToken cancellationToken)
    {
        var command = new CloseShiftCommand(shiftId, request);
        var response = await _messageBus.InvokeAsync<APIResponse<bool>>(command, cancellationToken);
        
        if (response.Success)
            return Ok(response);
            
        return BadRequest(response);
    }
}
