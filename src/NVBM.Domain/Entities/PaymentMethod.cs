using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NVBM.Domain.Entities;

public class PaymentMethod
{
    [Required]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty; // e.g., CASH, CARD, VNQRPAY
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsCash { get; set; } 
    public bool IsActive { get; set; } = true;
}
