namespace NVBM.Application.DTOs;

public class OpenShiftRequest
{
    public Guid UserId { get; set; }
    public string PosDeviceId { get; set; } = string.Empty;
    public decimal StartingCash { get; set; }
}

public class ShiftTransactionRequest
{
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CloseShiftRequest
{
    public decimal ActualCash { get; set; }
}

public class ShiftResponseDto
{
    public Guid Id { get; set; }
    public string PosDeviceId { get; set; } = string.Empty;
    public DateTime OpenTime { get; set; }
    public decimal StartingCash { get; set; }
    public string Status { get; set; } = string.Empty;
}
