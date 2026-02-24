using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

var optionsBuilder = new DbContextOptionsBuilder<NVBMDbContext>();
optionsBuilder.UseSqlServer("Data Source=BINHMINH-PC\\SQLEXPRESS;Database=NVBM_Grocery;Integrated Security=True;TrustServerCertificate=True;");

using var context = new NVBMDbContext(optionsBuilder.Options);

context.Database.ExecuteSqlRaw("DELETE FROM ProductBarcodes");
context.Database.ExecuteSqlRaw("DELETE FROM ProductAttributes");
context.Database.ExecuteSqlRaw("DELETE FROM ProductUoms");
context.Database.ExecuteSqlRaw("DELETE FROM InventoryLevels");
context.Database.ExecuteSqlRaw("DELETE FROM Products");
context.Database.ExecuteSqlRaw("DELETE FROM Categories");

if (true)
{
    Console.WriteLine("Seeding Sample Data...");

    // 1. Categories
    var catFresh = new Category { Id = Guid.NewGuid(), Name = "Thực phẩm tươi sống", Description = "Thịt cá, rau củ quả tươi" };
    var catDrink = new Category { Id = Guid.NewGuid(), Name = "Đồ uống", Description = "Nước giải khát, bia rượu" };
    var catSnack = new Category { Id = Guid.NewGuid(), Name = "Đồ ăn vặt", Description = "Bánh kẹo, snack" };

    context.Categories.AddRange(catFresh, catDrink, catSnack);

    // 2. Products
    var products = new List<Product>
    {
        new Product { Id = Guid.NewGuid(), CategoryId = catFresh.Id, Category = catFresh, Sku = "BEEF-01", Name = "Thịt Bò Úc Nhập Khẩu", Description = "Thịt bò thăn loại 1", IsActive = true },
        new Product { Id = Guid.NewGuid(), CategoryId = catFresh.Id, Category = catFresh, Sku = "SALMON-01", Name = "Cá Hồi Na Uy Cắt Lát", Description = "Cá hồi phi lê tươi", IsActive = true },
        new Product { Id = Guid.NewGuid(), CategoryId = catDrink.Id, Category = catDrink, Sku = "COKE-01", Name = "Coca Cola 330ml", Description = "Nước ngọt có gas", IsActive = true },
        new Product { Id = Guid.NewGuid(), CategoryId = catDrink.Id, Category = catDrink, Sku = "MILK-01", Name = "Sữa Tươi TH True Milk", Description = "Sữa tươi không đường 1L", IsActive = true },
        new Product { Id = Guid.NewGuid(), CategoryId = catSnack.Id, Category = catSnack, Sku = "CHIP-01", Name = "Snack Khoai Tây Lay's", Description = "Vị Tự nhiên 150g", IsActive = true }
    };

    context.Products.AddRange(products);

    // 3. UoMs (Units of Measure), Attributes, and Inventory
    foreach (var p in products)
    {
        // Generate UoM
        var basePrice = new Random().Next(15000, 300000); // Random price between 15k - 300k
        var uom = new ProductUom
        {
            Id = Guid.NewGuid(),
            ProductId = p.Id,
            UomCode = p.CategoryId == catFresh.Id ? "KG" : "ITEM",
            UomName = p.CategoryId == catFresh.Id ? "Kilogram" : "Cái/Lon/Gói",
            Price = basePrice,
            ConversionFactor = 1m
        };
        context.ProductUoms.Add(uom);

        // Generate Attribute
        var attr = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = p.Id,
            Key = "Xuất xứ",
            Value = p.CategoryId == catFresh.Id ? "Nhập khẩu" : "Việt Nam"
        };
        context.ProductAttributes.Add(attr);

        // Generate Barcode (e.g., SKU-123456)
        var randDigits = new Random().Next(100000, 999999);
        var barcodeStr = $"{p.Sku.Split('-')[0]}-{randDigits}";
        var barcode = new ProductBarcode
        {
            Barcode = barcodeStr,
            ProductId = p.Id,
            UomId = uom.Id
        };
        context.ProductBarcodes.Add(barcode);

        // Generate Inventory Level
        var inv = new InventoryLevel
        {
            ProductId = p.Id,
            AvailableQuantity = new Random().Next(50, 500),
            ReservedQuantity = 0
        };
        context.InventoryLevels.Add(inv);
    }

    await context.SaveChangesAsync();
    Console.WriteLine("Data seeding completed successfully!");
}
else 
{
    Console.WriteLine("Sample data already exists. Skipping seed.");
}

// Query and Display to confirm
var totalProducts = await context.Products.Where(p => p.IsActive).CountAsync();
Console.WriteLine($"Total Active Products in DB: {totalProducts}");

var data = await context.Products
    .Where(p => p.IsActive)
    .Include(p => p.Category)
    .Include(p => p.Uoms)
    .OrderByDescending(p => p.Name)
    .Take(5)
    .Select(p => new {
        p.Sku,
        p.Name,
        Category = p.Category!.Name,
        Price = p.Uoms.Any() ? p.Uoms.FirstOrDefault()!.Price : 0
    })
    .ToListAsync();

var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine("Sample 5 Latest Products:");
Console.WriteLine(json);
