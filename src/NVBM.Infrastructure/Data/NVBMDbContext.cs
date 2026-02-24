using Microsoft.EntityFrameworkCore;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Data;

public class NVBMDbContext : DbContext
{
    public NVBMDbContext(DbContextOptions<NVBMDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductUom> ProductUoms => Set<ProductUom>();
    public DbSet<ProductBarcode> ProductBarcodes => Set<ProductBarcode>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<InventoryLevel> InventoryLevels => Set<InventoryLevel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NVBMDbContext).Assembly);
    }
}
