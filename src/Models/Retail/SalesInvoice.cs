using LineManagementSystem.Models.Common;

namespace LineManagementSystem.Models.Retail;

public class SalesInvoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SalesInvoiceLine> Lines { get; set; } = new();
}

public class SalesInvoiceLine
{
    public int Id { get; set; }
    public int SalesInvoiceId { get; set; }
    public SalesInvoice? SalesInvoice { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
}

public enum PaymentMethod
{
    Cash,
    Card,
    Transfer,
    Mixed
}

public static class PaymentMethodExtensions
{
    public static string GetArabicName(this PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Cash => "نقدي",
            PaymentMethod.Card => "بطاقة",
            PaymentMethod.Transfer => "تحويل",
            PaymentMethod.Mixed => "مختلط",
            _ => method.ToString()
        };
    }
}
