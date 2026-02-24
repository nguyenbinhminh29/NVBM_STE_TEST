using NVBM.Application.DTOs;

namespace NVBM.Application.Interfaces;

public interface IBarcodeService
{
    Task<BarcodeLookupResponse?> LookupAsync(string barcode);
}
