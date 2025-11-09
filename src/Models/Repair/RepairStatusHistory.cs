namespace LineManagementSystem.Models.Repair;

public class RepairStatusHistory
{
    public int Id { get; set; }
    public int RepairOrderId { get; set; }
    public RepairOrder? RepairOrder { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;
}
