using System.ComponentModel.DataAnnotations;

namespace NVBM.Domain.Entities;

public class ShiftTransaction
{
    public Guid Id { get; set; }

    public Guid ShiftId { get; set; }
    public Shift? Shift { get; set; }

    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // PAY_IN, PAY_OUT

    public decimal Amount { get; set; }

    public DateTime TransactionTime { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
