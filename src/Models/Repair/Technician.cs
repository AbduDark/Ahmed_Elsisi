namespace LineManagementSystem.Models.Repair;

public class Technician
{
    public int Id { get; set; }
    public string NameArabic { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime HireDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }

    public List<RepairOrder> Repairs { get; set; } = new();
}
