namespace LineManagementSystem.Models.Common;

public class Customer
{
    public int Id { get; set; }
    public string NameArabic { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public CustomerType CustomerType { get; set; } = CustomerType.Individual;
    public string? Notes { get; set; }
    public decimal TotalPurchases { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum CustomerType
{
    Individual,
    Business
}
