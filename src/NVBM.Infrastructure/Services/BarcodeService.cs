using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Services;

public class BarcodeService : IBarcodeService
{
    private readonly NVBMDbContext _dbContext;

    public BarcodeService(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BarcodeLookupResponse?> LookupAsync(string barcode)
    {
        // Tra cứu 3 bảng với AsNoTracking() để tối ưu hiệu năng đọc
        return await _dbContext.ProductBarcodes
            .AsNoTracking()
            .Where(b => b.Barcode == barcode)
            .Select(b => new BarcodeLookupResponse
            {
                Barcode = b.Barcode,
                Sku = b.Product!.Sku,
                Name = b.Product.Name,
                UomCode = b.Uom!.UomCode,
                UomName = b.Uom.UomName,
                Price = b.Uom.Price,
                ConversionFactor = b.Uom.ConversionFactor,
                ProductId = b.ProductId,
                UomId = b.UomId
            })
            .FirstOrDefaultAsync();
    }
}
