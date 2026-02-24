using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sku).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion();
        
        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => x.CategoryId);

        builder.HasOne(x => x.Category)
               .WithMany(x => x.Products)
               .HasForeignKey(x => x.CategoryId);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
    }
}

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.Product)
               .WithMany(x => x.Attributes)
               .HasForeignKey(x => x.ProductId);
    }
}

public class ProductUomConfiguration : IEntityTypeConfiguration<ProductUom>
{
    public void Configure(EntityTypeBuilder<ProductUom> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UomCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.ConversionFactor).HasPrecision(18, 4);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Product)
               .WithMany(x => x.Uoms)
               .HasForeignKey(x => x.ProductId);
    }
}

public class ProductBarcodeConfiguration : IEntityTypeConfiguration<ProductBarcode>
{
    public void Configure(EntityTypeBuilder<ProductBarcode> builder)
    {
        builder.HasKey(x => x.Barcode);
        builder.Property(x => x.Barcode).IsRequired().HasMaxLength(50);
        
        // Explicitly set non-clustered if another clustered index was meant, but PK is clustered by default.
        // Let's just avoid adding an explicit unique clustered index since PK is it.

        builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Uom)
               .WithMany()
               .HasForeignKey(x => x.UomId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
