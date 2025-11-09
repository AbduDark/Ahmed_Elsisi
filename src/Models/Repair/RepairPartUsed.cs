using LineManagementSystem.Models.Retail;

namespace LineManagementSystem.Models.Repair;

public class RepairPartUsed
{
    public int Id { get; set; }
    public int RepairOrderId { get; set; }
    public RepairOrder? RepairOrder { get; set; }
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
}
