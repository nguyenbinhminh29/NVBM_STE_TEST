using System.ComponentModel.DataAnnotations;

namespace NVBM.Domain.Entities;

public class Shift
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string PosDeviceId { get; set; } = string.Empty;

    public DateTime OpenTime { get; set; } = DateTime.UtcNow;
    public DateTime? CloseTime { get; set; }

    public decimal StartingCash { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal? ActualCash { get; set; }

    public decimal TotalSalesAmount { get; set; } // CQRS Projected Field
    public decimal TotalCashReceived { get; set; } // CQRS Projected Field

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "OPEN"; // OPEN, CLOSED

    public ICollection<ShiftTransaction> Transactions { get; set; } = new List<ShiftTransaction>();
}
