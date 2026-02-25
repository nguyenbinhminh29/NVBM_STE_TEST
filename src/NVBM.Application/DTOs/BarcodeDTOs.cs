namespace NVBM.Application.DTOs;

public record BarcodeLookupResponse
{
    public string Barcode { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string UomCode { get; init; } = string.Empty;
    public string UomName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal ConversionFactor { get; init; }
    public Guid ProductId { get; init; }
    public Guid UomId { get; init; }
}
