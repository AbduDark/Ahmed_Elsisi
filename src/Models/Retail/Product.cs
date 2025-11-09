namespace LineManagementSystem.Models.Retail;

public class Product
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string NameArabic { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public ProductCategory Category { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; } = 5;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsLowStock => CurrentStock <= MinimumStock;
    public decimal ProfitMargin => SalePrice - PurchasePrice;
    public decimal ProfitPercentage => PurchasePrice > 0 ? ((SalePrice - PurchasePrice) / PurchasePrice * 100) : 0;
}

public enum ProductCategory
{
    Covers,
    Chargers,
    Headphones,
    ScreenProtectors,
    Cables,
    PowerBanks,
    Holders,
    MemoryCards,
    Other
}

public static class ProductCategoryExtensions
{
    public static string GetArabicName(this ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Covers => "أغطية",
            ProductCategory.Chargers => "شواحن",
            ProductCategory.Headphones => "سماعات",
            ProductCategory.ScreenProtectors => "حماية شاشة",
            ProductCategory.Cables => "كابلات",
            ProductCategory.PowerBanks => "باوربنك",
            ProductCategory.Holders => "حوامل",
            ProductCategory.MemoryCards => "كروت ذاكرة",
            ProductCategory.Other => "أخرى",
            _ => category.ToString()
        };
    }
}
