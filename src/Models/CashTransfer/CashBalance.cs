namespace LineManagementSystem.Models.CashTransfer;

public class CashBalance
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public CashProvider? Provider { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

public class BalanceHistory
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public CashProvider? Provider { get; set; }
    public int? TransactionId { get; set; }
    public CashTransferTransaction? Transaction { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
