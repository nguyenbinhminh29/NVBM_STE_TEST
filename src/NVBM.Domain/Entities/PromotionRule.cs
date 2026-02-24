using System.ComponentModel.DataAnnotations;

namespace NVBM.Domain.Entities;

public class PromotionRule
{
    public Guid Id { get; set; }
    
    public Guid PromotionId { get; set; }
    public Promotion? Promotion { get; set; }

    [Required]
    [MaxLength(50)]
    public string RuleType { get; set; } = string.Empty;

    public string RulePayload { get; set; } = string.Empty; // JSON
}
