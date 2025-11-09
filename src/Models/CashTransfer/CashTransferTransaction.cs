namespace LineManagementSystem.Models.CashTransfer;

public class CashTransferTransaction
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public CashProvider? Provider { get; set; }
    public CashTransactionType TransactionType { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public CashTransactionStatus Status { get; set; } = CashTransactionStatus.Pending;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum CashTransactionType
{
    Deposit,
    Withdrawal,
    Transfer
}

public enum CashTransactionStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}

public static class CashTransactionTypeExtensions
{
    public static string GetArabicName(this CashTransactionType type)
    {
        return type switch
        {
            CashTransactionType.Deposit => "إيداع",
            CashTransactionType.Withdrawal => "سحب",
            CashTransactionType.Transfer => "تحويل",
            _ => type.ToString()
        };
    }
}

public static class CashTransactionStatusExtensions
{
    public static string GetArabicName(this CashTransactionStatus status)
    {
        return status switch
        {
            CashTransactionStatus.Pending => "معلق",
            CashTransactionStatus.Completed => "مكتمل",
            CashTransactionStatus.Failed => "فاشل",
            CashTransactionStatus.Cancelled => "ملغي",
            _ => status.ToString()
        };
    }

    public static string GetColorHex(this CashTransactionStatus status)
    {
        return status switch
        {
            CashTransactionStatus.Pending => "#FF9800",
            CashTransactionStatus.Completed => "#4CAF50",
            CashTransactionStatus.Failed => "#F44336",
            CashTransactionStatus.Cancelled => "#9E9E9E",
            _ => "#000000"
        };
    }
}
