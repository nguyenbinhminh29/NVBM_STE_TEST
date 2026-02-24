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
}

public record APIResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;

    public static APIResponse<T> Ok(T data, string message = "Success") => new() { Success = true, Data = data, Message = message };
    public static APIResponse<T> Fail(string message) => new() { Success = false, Message = message };
}
