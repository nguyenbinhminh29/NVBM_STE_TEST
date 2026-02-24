namespace NVBM.Domain.Entities;

public class InventoryLevel
{
    public Guid ProductId { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Product? Product { get; set; }
}
