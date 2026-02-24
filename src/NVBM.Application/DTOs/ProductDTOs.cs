using FluentValidation;

namespace NVBM.Application.DTOs;

public record CreateProductDto
{
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public List<CreateProductUomDto> Uoms { get; init; } = new();
    public List<CreateProductAttributeDto> Attributes { get; init; } = new();
}

public record CreateProductUomDto
{
    public string UomCode { get; init; } = string.Empty;
    public string UomName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal ConversionFactor { get; init; }
}

public record CreateProductAttributeDto
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public record UpdateProductDto : CreateProductDto
{
}

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Attributes)
            .Must(attr => attr.Select(a => a.Key).Distinct().Count() == attr.Count)
            .WithMessage("Attribute keys must be unique.")
            .Must(attr => attr.Count <= 100)
            .WithMessage("Maximum 100 attributes allowed.");
    }
}

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
    public string BaseUom { get; init; } = string.Empty;
    public List<ProductUomDto> AvailableUnits { get; init; } = new();
    public List<ProductAttributeDto> Attributes { get; init; } = new();
}

public record ProductUomDto
{
    public Guid Id { get; init; }
    public string UomCode { get; init; } = string.Empty;
    public string UomName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal ConversionFactor { get; init; }
    public bool IsDefault { get; init; }
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


