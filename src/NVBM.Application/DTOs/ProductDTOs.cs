namespace NVBM.Application.DTOs;

public record ProductListDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public decimal MinPrice { get; init; }
}

public record ProductDetailDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<ProductUomDto> Uoms { get; init; } = new();
    public List<ProductAttributeDto> Attributes { get; init; } = new();
}

public record ProductUomDto
{
    public Guid Id { get; init; }
    public string UomCode { get; init; } = string.Empty;
    public string UomName { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

public record ProductAttributeDto
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public record PagedResponse<T>
{
    public List<T> Items { get; init; } = new();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
