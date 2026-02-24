namespace NVBM.Application.DTOs;

public class OrderCalculateRequest
{
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCalculateResponse
{
    public decimal SubTotal { get; set; } // Tổng tiền trước giảm
    public decimal TotalDiscount { get; set; } // Tổng tiền giảm giá
    public decimal GrandTotal { get; set; } // Tổng tiền phải thu

    public List<OrderLineResponseDto> Lines { get; set; } = new();
}

public class OrderLineResponseDto
{
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string UomName { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public decimal OriginalLineTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalLineTotal { get; set; }
    
    // Lý do được giảm giá (Tên khuyến mãi)
    public List<string> AppliedPromotions { get; set; } = new();
}
