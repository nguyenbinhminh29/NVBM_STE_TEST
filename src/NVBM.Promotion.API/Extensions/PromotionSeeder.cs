using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;
using NVBM.Infrastructure.Services;

namespace NVBM.Promotion.API.Extensions;

public static class PromotionSeeder
{
    public static async Task SeedPromotionDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NVBMDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Promotions.AnyAsync())
        {
            context.Promotions.RemoveRange(context.Promotions);
            await context.SaveChangesAsync();
        }

        // Example: Discount 10,000VND if buying specific item
        // Product ID provided from existing database
        var sampleProductId = Guid.Parse("C0339AFD-3217-43A3-A4DD-895B22733BE4"); 

        var promotion1 = new NVBM.Domain.Entities.Promotion
        {
            Id = Guid.NewGuid(),
            Name = "MUA NỀN - GIẢM MẠNH: Giảm 10K khi mua 3 lốc sữa",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            IsStackable = false, // Stop other promotions
            Rules = new List<PromotionRule>
            {
                new PromotionRule
                {
                    Id = Guid.NewGuid(),
                    RuleType = "MIN_ITEM_QTY",
                    RulePayload = JsonSerializer.Serialize(new MinItemQtyRulePayload
                    {
                        ProductId = sampleProductId,
                        MinQuantity = 3
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                }
            },
            Actions = new List<PromotionAction>
            {
                new PromotionAction
                {
                    Id = Guid.NewGuid(),
                    ActionType = "ITEM_FIXED_DISCOUNT",
                    ActionPayload = JsonSerializer.Serialize(new ItemFixedDiscountPayload
                    {
                        TargetProductId = sampleProductId,
                        DiscountValue = 10000m
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                }
            }
        };

        var promotion2 = new NVBM.Domain.Entities.Promotion
        {
            Id = Guid.NewGuid(),
            Name = "FLASH SALE TRƯA HÈ: Mua 1 Mỳ Gói giá Tốt nhất", // Using another unknown product
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            IsStackable = true,
            Rules = new List<PromotionRule>
            {
                new PromotionRule
                {
                    Id = Guid.NewGuid(),
                    RuleType = "MIN_ITEM_QTY",
                    RulePayload = JsonSerializer.Serialize(new MinItemQtyRulePayload
                    {
                        ProductId = Guid.NewGuid(),
                        MinQuantity = 1
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                }
            },
            Actions = new List<PromotionAction>
            {
                new PromotionAction
                {
                    Id = Guid.NewGuid(),
                    ActionType = "ITEM_FIXED_DISCOUNT",
                    ActionPayload = JsonSerializer.Serialize(new ItemFixedDiscountPayload
                    {
                        TargetProductId = Guid.NewGuid(),
                        DiscountValue = 3000m
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                }
            }
        };

        context.Promotions.AddRange(promotion1, promotion2);
        await context.SaveChangesAsync();
    }
}
