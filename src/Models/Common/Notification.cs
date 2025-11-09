namespace LineManagementSystem.Models.Common;

public class Notification
{
    public int Id { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public bool IsRead { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum NotificationType
{
    LowStock,
    RepairDue,
    PaymentDue,
    LineRenewal,
    LowBalance,
    SystemAlert
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}

public static class NotificationTypeExtensions
{
    public static string GetArabicName(this NotificationType type)
    {
        return type switch
        {
            NotificationType.LowStock => "مخزون منخفض",
            NotificationType.RepairDue => "صيانة متأخرة",
            NotificationType.PaymentDue => "دفعة مستحقة",
            NotificationType.LineRenewal => "تجديد خطوط",
            NotificationType.LowBalance => "رصيد منخفض",
            NotificationType.SystemAlert => "تنبيه نظام",
            _ => type.ToString()
        };
    }

    public static string GetIcon(this NotificationType type)
    {
        return type switch
        {
            NotificationType.LowStock => "📦",
            NotificationType.RepairDue => "🔧",
            NotificationType.PaymentDue => "💰",
            NotificationType.LineRenewal => "📞",
            NotificationType.LowBalance => "⚠️",
            NotificationType.SystemAlert => "🔔",
            _ => "ℹ️"
        };
    }
}
