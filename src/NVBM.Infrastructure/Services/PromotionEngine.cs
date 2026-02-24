using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Services;

public class PromotionEngine : IPromotionEngine
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "ActivePromotions";

    public PromotionEngine(
        IPromotionRepository promotionRepository, 
        IProductRepository productRepository, 
        IMemoryCache cache)
    {
        _promotionRepository = promotionRepository;
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<OrderCalculateResponse> CalculateOrderAsync(OrderCalculateRequest request, CancellationToken cancellationToken = default)
    {
        var response = new OrderCalculateResponse();
        
        // 1. Load active promotions from cache or DB
        var activePromotions = await GetActivePromotionsFromCacheAsync(cancellationToken);

        // 2. Fetch full product pricing
        var productDict = new Dictionary<Guid, (string Name, string UomName, decimal Price)>();
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(item.ProductId, true, cancellationToken);
            if (product != null)
            {
                var uom = product.Uoms.FirstOrDefault(u => u.Id == item.UomId);
                if (uom != null)
                {
                    productDict[item.ProductId] = (product.Name, uom.UomName, uom.Price);
                    
                    var line = new OrderLineResponseDto
                    {
                        ProductId = item.ProductId,
                        UomId = item.UomId,
                        ProductName = product.Name,
                        UomName = uom.UomName,
                        Quantity = item.Quantity,
                        UnitPrice = uom.Price,
                        OriginalLineTotal = uom.Price * item.Quantity,
                        FinalLineTotal = uom.Price * item.Quantity
                    };
                    response.Lines.Add(line);
                }
            }
        }

        // 3. Rule Evaluator & Action Applier
        // Evaluate each promotion rules
        foreach (var promotion in activePromotions)
        {
            ApplyPromotionToCart(promotion, response);
        }

        // 4. Summarize totals
        response.SubTotal = response.Lines.Sum(l => l.OriginalLineTotal);
        response.TotalDiscount = response.Lines.Sum(l => l.DiscountAmount);
        response.GrandTotal = response.Lines.Sum(l => l.FinalLineTotal);

        return response;
    }

    private void ApplyPromotionToCart(Promotion promotion, OrderCalculateResponse cart)
    {
        // Simple Eligibility Pipeline based on Document
        bool isEligible = true;

        foreach (var rule in promotion.Rules)
        {
            if (rule.RuleType == "MIN_ITEM_QTY")
            {
                // Parse Payload {"productId": "xxx", "minQuantity": 3}
                var payload = JsonSerializer.Deserialize<MinItemQtyRulePayload>(rule.RulePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                if (payload != null)
                {
                    var matchingLine = cart.Lines.FirstOrDefault(l => l.ProductId == payload.ProductId);
                    if (matchingLine == null || matchingLine.Quantity < payload.MinQuantity)
                    {
                        isEligible = false;
                        break;
                    }
                }
            }
            // other rules can be added here
        }

        if (isEligible)
        {
            // Apply Actions
            foreach (var action in promotion.Actions)
            {
                if (action.ActionType == "ITEM_FIXED_DISCOUNT")
                {
                    var actionPayload = JsonSerializer.Deserialize<ItemFixedDiscountPayload>(action.ActionPayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (actionPayload != null)
                    {
                        // Assume we apply fixed discount to the item from the rule constraint or specific item
                        // For simplicity, finding any item to apply it to if applicable.
                        // Or we can say action payload has TargetProductId
                        var targetLine = cart.Lines.FirstOrDefault(l => l.ProductId == actionPayload.TargetProductId);
                        if (targetLine != null)
                        {
                            // 4. Conflict Resolver: Check IsStackable
                            if (!promotion.IsStackable && targetLine.AppliedPromotions.Any())
                            {
                                continue; // Skip if it already has discount and new promo goes against IsStackable policy
                            }

                            decimal potentialDiscount = actionPayload.DiscountValue;
                            if (potentialDiscount > targetLine.FinalLineTotal)
                            {
                                potentialDiscount = targetLine.FinalLineTotal; 
                            }
                            
                            targetLine.DiscountAmount += potentialDiscount;
                            targetLine.FinalLineTotal -= potentialDiscount;
                            targetLine.AppliedPromotions.Add(promotion.Name);
                        }
                    }
                }
            }
        }
    }

    private async Task<List<Promotion>> GetActivePromotionsFromCacheAsync(CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue(CacheKey, out List<Promotion>? cachedPromotions))
        {
            cachedPromotions = await _promotionRepository.GetActivePromotionsAsync(DateTime.UtcNow, cancellationToken);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _cache.Set(CacheKey, cachedPromotions, cacheEntryOptions);
        }

        return cachedPromotions ?? new List<Promotion>();
    }
}

// Payload Classes for Rule Engine (Simulation of NoSQL JSON parsing)
public class MinItemQtyRulePayload
{
    public Guid ProductId { get; set; }
    public int MinQuantity { get; set; }
}

public class ItemFixedDiscountPayload
{
    public Guid TargetProductId { get; set; }
    public decimal DiscountValue { get; set; }
}
