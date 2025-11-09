namespace LineManagementSystem.Models.Retail;

public class Supplier
{
    public int Id { get; set; }
    public string NameArabic { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
