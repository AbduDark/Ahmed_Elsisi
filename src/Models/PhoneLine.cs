using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace LineManagementSystem.Models;

public class PhoneLine : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string InternalId { get; set; } = string.Empty;

    public bool HasCashWallet { get; set; }

    [MaxLength(50)]
    public string? CashWalletNumber { get; set; }

    [MaxLength(500)]
    public string? Details { get; set; }

    [MaxLength(100)]
    public string? LineSystem { get; set; }

    private int _confirmationLevel = 0;
    public int ConfirmationLevel
    {
        get => _confirmationLevel;
        set
        {
            if (_confirmationLevel != value)
            {
                _confirmationLevel = value;
                OnPropertyChanged();
            }
        }
    }

    public int GroupId { get; set; }
    public LineGroup? Group { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
