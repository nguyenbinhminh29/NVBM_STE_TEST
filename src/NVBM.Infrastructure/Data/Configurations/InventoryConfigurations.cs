using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class InventoryLevelConfiguration : IEntityTypeConfiguration<InventoryLevel>
{
    public void Configure(EntityTypeBuilder<InventoryLevel> builder)
    {
        builder.HasKey(x => x.ProductId);
        
        builder.Property(x => x.AvailableQuantity).HasPrecision(18, 2);
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 2);
        
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Product)
               .WithOne()
               .HasForeignKey<InventoryLevel>(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
