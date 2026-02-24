namespace NVBM.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<ProductUom> Uoms { get; set; } = new List<ProductUom>();
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
}

public class ProductUom
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string UomCode { get; set; } = string.Empty;
    public string UomName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal ConversionFactor { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public Product? Product { get; set; }
}

public class ProductBarcode
{
    public string Barcode { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public Product? Product { get; set; }
    public ProductUom? Uom { get; set; }
}
