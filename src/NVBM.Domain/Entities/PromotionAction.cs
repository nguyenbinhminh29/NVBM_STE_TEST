using System.ComponentModel.DataAnnotations;

namespace NVBM.Domain.Entities;

public class PromotionAction
{
    public Guid Id { get; set; }
    
    public Guid PromotionId { get; set; }
    public Promotion? Promotion { get; set; }

    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty;

    public string ActionPayload { get; set; } = string.Empty; // JSON
}
