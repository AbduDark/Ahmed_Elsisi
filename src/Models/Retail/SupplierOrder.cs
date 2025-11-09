namespace LineManagementSystem.Models.Retail;

public class SupplierOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public SupplierOrderStatus Status { get; set; } = SupplierOrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<SupplierOrderLine> Lines { get; set; } = new();
}

public class SupplierOrderLine
{
    public int Id { get; set; }
    public int SupplierOrderId { get; set; }
    public SupplierOrder? SupplierOrder { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public enum SupplierOrderStatus
{
    Pending,
    Received,
    PartiallyReceived,
    Cancelled
}

public static class SupplierOrderStatusExtensions
{
    public static string GetArabicName(this SupplierOrderStatus status)
    {
        return status switch
        {
            SupplierOrderStatus.Pending => "معلق",
            SupplierOrderStatus.Received => "مستلم",
            SupplierOrderStatus.PartiallyReceived => "مستلم جزئياً",
            SupplierOrderStatus.Cancelled => "ملغي",
            _ => status.ToString()
        };
    }
}
