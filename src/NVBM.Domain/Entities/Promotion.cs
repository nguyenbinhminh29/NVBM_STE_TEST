using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NVBM.Domain.Entities;

public class Promotion
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsStackable { get; set; } = false;

    public ICollection<PromotionRule> Rules { get; set; } = new List<PromotionRule>();
    public ICollection<PromotionAction> Actions { get; set; } = new List<PromotionAction>();
}
