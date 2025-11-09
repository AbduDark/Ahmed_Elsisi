namespace LineManagementSystem.Models.CashTransfer;

public class CashProvider
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderCode { get; set; } = string.Empty;
    public decimal Commission { get; set; }
    public decimal MinTransfer { get; set; }
    public decimal MaxTransfer { get; set; }
    public bool IsActive { get; set; } = true;

    public List<CashTransferTransaction> Transactions { get; set; } = new();
}
