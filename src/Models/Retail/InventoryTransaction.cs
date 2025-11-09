namespace LineManagementSystem.Models.Retail;

public class InventoryTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public TransactionType TransactionType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum TransactionType
{
    Purchase,
    Sale,
    Adjustment,
    Return
}

public static class TransactionTypeExtensions
{
    public static string GetArabicName(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Purchase => "شراء",
            TransactionType.Sale => "بيع",
            TransactionType.Adjustment => "تعديل",
            TransactionType.Return => "مرتجع",
            _ => type.ToString()
        };
    }
}
