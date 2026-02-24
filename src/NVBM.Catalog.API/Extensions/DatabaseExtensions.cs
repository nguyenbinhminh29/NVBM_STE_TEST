using Microsoft.EntityFrameworkCore;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Catalog.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NVBMDbContext>();

        // Apply Migrations
        await dbContext.Database.MigrateAsync();

        // Seed Data
        await SeedDataAsync(dbContext);
    }

    private static async Task SeedDataAsync(NVBMDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return; // Already seeded

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Beverages",
            Description = "Drinks and liquids."
        };
        await context.Categories.AddAsync(category);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CategoryId = category.Id,
            Sku = "BEER-HEIN-01",
            Name = "Bia Heineken Nhập khẩu",
            Description = "Bia Heineken lon nhập khẩu, thơm ngon.",
            Category = category
        };
        await context.Products.AddAsync(product);

        var uomLon = new ProductUom
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomCode = "CAN",
            UomName = "Lon 330ml",
            Price = 20000m,
            ConversionFactor = 1m,
            Product = product
        };
        await context.ProductUoms.AddAsync(uomLon);

        var uomThung = new ProductUom
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomCode = "BOX",
            UomName = "Thùng 24 Lon",
            Price = 450000m,
            ConversionFactor = 24m,
            Product = product
        };
        await context.ProductUoms.AddAsync(uomThung);

        var barcode1 = new ProductBarcode
        {
            Barcode = "8934567890123",
            ProductId = product.Id,
            UomId = uomLon.Id,
            Product = product,
            Uom = uomLon
        };
        await context.ProductBarcodes.AddAsync(barcode1);

        var attribute1 = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Key = "Brand",
            Value = "Heineken",
            Product = product
        };
        var attribute2 = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Key = "Origin",
            Value = "Netherlands",
            Product = product
        };
        await context.ProductAttributes.AddRangeAsync(attribute1, attribute2);

        await context.SaveChangesAsync();
    }
}
