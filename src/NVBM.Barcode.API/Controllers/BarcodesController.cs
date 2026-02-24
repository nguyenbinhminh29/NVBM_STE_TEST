using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;

namespace NVBM.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[OutputCache(Duration = 300)]
public class BarcodesController : ControllerBase
{
    private readonly IBarcodeService _barcodeService;

    public BarcodesController(IBarcodeService barcodeService)
    {
        _barcodeService = barcodeService;
    }

    [HttpGet("{barcode}/lookup")]
    public async Task<ActionResult<APIResponse<BarcodeLookupResponse>>> Lookup(string barcode)
    {
        var result = await _barcodeService.LookupAsync(barcode);

        if (result == null)
        {
            return NotFound(APIResponse<BarcodeLookupResponse>.Fail("Barcode not found"));
        }

        return Ok(APIResponse<BarcodeLookupResponse>.Ok(result, "Lookup successful"));
    }
}
