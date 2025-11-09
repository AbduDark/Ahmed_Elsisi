using LineManagementSystem.Models.Common;

namespace LineManagementSystem.Models.Repair;

public class RepairOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string? IMEI { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public RepairStatus Status { get; set; } = RepairStatus.Received;
    public RepairPriority Priority { get; set; } = RepairPriority.Normal;
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Profit => SellingPrice - ActualCost;
    public int? TechnicianId { get; set; }
    public Technician? Technician { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.Now;
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<RepairPartUsed> PartsUsed { get; set; } = new();
    public List<RepairStatusHistory> StatusHistory { get; set; } = new();
}

public enum RepairStatus
{
    Received,
    InProgress,
    WaitingParts,
    Completed,
    Delivered,
    Cancelled
}

public enum RepairPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public static class RepairStatusExtensions
{
    public static string GetArabicName(this RepairStatus status)
    {
        return status switch
        {
            RepairStatus.Received => "مستلم",
            RepairStatus.InProgress => "قيد العمل",
            RepairStatus.WaitingParts => "بانتظار القطع",
            RepairStatus.Completed => "منتهي",
            RepairStatus.Delivered => "مُسلّم",
            RepairStatus.Cancelled => "ملغي",
            _ => status.ToString()
        };
    }

    public static string GetColorHex(this RepairStatus status)
    {
        return status switch
        {
            RepairStatus.Received => "#2196F3",
            RepairStatus.InProgress => "#FF9800",
            RepairStatus.WaitingParts => "#9C27B0",
            RepairStatus.Completed => "#4CAF50",
            RepairStatus.Delivered => "#8BC34A",
            RepairStatus.Cancelled => "#F44336",
            _ => "#000000"
        };
    }
}

public static class RepairPriorityExtensions
{
    public static string GetArabicName(this RepairPriority priority)
    {
        return priority switch
        {
            RepairPriority.Low => "منخفضة",
            RepairPriority.Normal => "عادية",
            RepairPriority.High => "عالية",
            RepairPriority.Urgent => "عاجلة",
            _ => priority.ToString()
        };
    }
}
