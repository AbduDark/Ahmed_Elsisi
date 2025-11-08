
using System.Collections.ObjectModel;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.ViewModels;

public class SearchViewModel : BaseViewModel
{
    private readonly GroupService _groupService;
    private readonly DatabaseContext _context;
    private string _searchQuery = string.Empty;
    private string _searchType = "الكل";
    private string _selectedProvider = "الكل";
    private string _selectedStatus = "الكل";
    private string _selectedCashWallet = "الكل";

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            SetProperty(ref _searchQuery, value);
            SearchCommand.Execute(null);
        }
    }

    public string SearchType
    {
        get => _searchType;
        set
        {
            SetProperty(ref _searchType, value);
            SearchCommand.Execute(null);
        }
    }

    public string SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            SetProperty(ref _selectedProvider, value);
            SearchCommand.Execute(null);
        }
    }

    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            SetProperty(ref _selectedStatus, value);
            SearchCommand.Execute(null);
        }
    }

    public string SelectedCashWallet
    {
        get => _selectedCashWallet;
        set
        {
            SetProperty(ref _selectedCashWallet, value);
            SearchCommand.Execute(null);
        }
    }

    public ObservableCollection<SearchResult> SearchResults { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand OpenItemCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand EditItemCommand { get; }

    public event Action<LineGroup>? NavigateToGroupDetails;
    public event Action<PhoneLine, LineGroup>? EditLine;
    public event Action<int>? DeleteLine;
    public event Action<int>? DeleteGroup;

    public SearchViewModel(GroupService groupService, DatabaseContext context)
    {
        _groupService = groupService;
        _context = context;

        SearchCommand = new RelayCommand(() =>
        {
            PerformSearch();
        });

        ClearCommand = new RelayCommand(() =>
        {
            SearchQuery = string.Empty;
            SearchResults.Clear();
        });

        OpenItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            if (result.Type == "مجموعة")
            {
                // فتح تفاصيل المجموعة
                var group = _context.LineGroups
                    .Include(g => g.Lines)
                    .FirstOrDefault(g => g.Name == result.GroupName);
                
                if (group != null)
                {
                    NavigateToGroupDetails?.Invoke(group);
                }
            }
            else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
            {
                // فتح تعديل الخط
                var line = _context.PhoneLines
                    .Include(l => l.Group)
                    .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                
                if (line != null && line.Group != null)
                {
                    EditLine?.Invoke(line, line.Group);
                }
            }
        });

        DeleteItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            var confirmResult = System.Windows.MessageBox.Show(
                $"هل أنت متأكد من حذف {result.Type}: {(result.Type == "مجموعة" ? result.GroupName : result.Name)}؟",
                "تأكيد الحذف",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (confirmResult == System.Windows.MessageBoxResult.Yes)
            {
                // تنظيف الـ tracker قبل الحذف
                _context.ChangeTracker.Clear();
                
                if (result.Type == "مجموعة")
                {
                    var group = _context.LineGroups.FirstOrDefault(g => g.Name == result.GroupName);
                    if (group != null)
                    {
                        DeleteGroup?.Invoke(group.Id);
                        PerformSearch();
                    }
                }
                else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
                {
                    var line = _context.PhoneLines
                        .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                    if (line != null)
                    {
                        DeleteLine?.Invoke(line.Id);
                        PerformSearch();
                    }
                }
            }
        });

        EditItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            if (result.Type == "مجموعة")
            {
                var group = _context.LineGroups
                    .Include(g => g.Lines)
                    .FirstOrDefault(g => g.Name == result.GroupName);
                
                if (group != null)
                {
                    NavigateToGroupDetails?.Invoke(group);
                }
            }
            else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
            {
                var line = _context.PhoneLines
                    .Include(l => l.Group)
                    .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                
                if (line != null && line.Group != null)
                {
                    EditLine?.Invoke(line, line.Group);
                }
            }
        });
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        var query = SearchQuery.Trim().ToLower();

        // البحث في الخطوط مع إصلاح منطق الشروط
        var linesQuery = _context.PhoneLines
            .Include(l => l.Group)
            .AsQueryable();

        // تطبيق فلتر البحث النصي
        linesQuery = linesQuery.Where(l =>
            ((SearchType == "الكل" || SearchType == "رقم قومي") && l.NationalId.ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "اسم الشخص") && l.Name.ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "رقم الخط") && l.PhoneNumber.ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "رقم داخلي") && l.InternalId.ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "محفظة كاش") && (l.CashWalletNumber ?? "").ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "نظام الخط") && (l.LineSystem ?? "").ToLower().Contains(query)) ||
            ((SearchType == "الكل" || SearchType == "التفاصيل") && (l.Details ?? "").ToLower().Contains(query))
        );

        // تطبيق فلتر الشركة
        if (SelectedProvider != "الكل")
        {
            TelecomProvider provider = SelectedProvider switch
            {
                "فودافون" => TelecomProvider.Vodafone,
                "اتصالات" => TelecomProvider.Etisalat,
                "وي" => TelecomProvider.We,
                "أورانج" => TelecomProvider.Orange,
                _ => TelecomProvider.Vodafone
            };
            linesQuery = linesQuery.Where(l => l.Group != null && l.Group.Provider == provider);
        }

        // تطبيق فلتر حالة المجموعة
        if (SelectedStatus != "الكل")
        {
            GroupStatus status = SelectedStatus switch
            {
                "نشط" => GroupStatus.Active,
                "موقوف" => GroupStatus.Suspended,
                "محظور" => GroupStatus.Barred,
                "بدون محفظة" => GroupStatus.Cashless,
                "بمحفظة" => GroupStatus.CashWallet,
                _ => GroupStatus.Active
            };
            linesQuery = linesQuery.Where(l => l.Group != null && l.Group.Status == status);
        }

        // تطبيق فلتر محفظة الكاش
        if (SelectedCashWallet == "نعم")
        {
            linesQuery = linesQuery.Where(l => l.HasCashWallet);
        }
        else if (SelectedCashWallet == "لا")
        {
            linesQuery = linesQuery.Where(l => !l.HasCashWallet);
        }

        var lines = linesQuery.ToList();

        foreach (var line in lines)
        {
            SearchResults.Add(new SearchResult
            {
                Type = "خط",
                Name = line.Name,
                PhoneNumber = line.PhoneNumber,
                NationalId = line.NationalId,
                InternalId = line.InternalId,
                GroupName = line.Group?.Name ?? "غير محدد",
                Provider = line.Group?.Provider.GetArabicName() ?? "غير محدد",
                ProviderColor = line.Group?.Provider.GetColorHex() ?? "#000000",
                CashWalletNumber = line.CashWalletNumber,
                HasCashWallet = line.HasCashWallet,
                Details = line.Details
            });
        }

        // البحث في المجموعات
        if (SearchType == "الكل" || SearchType == "اسم المجموعة" || SearchType == "موظف مسؤول" || SearchType == "عميل" || SearchType == "تفاصيل إضافية")
        {
            var groupsQuery = _context.LineGroups
                .Include(g => g.Lines)
                .AsQueryable();

            // تطبيق فلتر البحث النصي
            groupsQuery = groupsQuery.Where(g => 
                ((SearchType == "الكل" || SearchType == "اسم المجموعة") && g.Name.ToLower().Contains(query)) ||
                ((SearchType == "الكل" || SearchType == "موظف مسؤول") && (g.AssignedToEmployee ?? "").ToLower().Contains(query)) ||
                ((SearchType == "الكل" || SearchType == "عميل") && (g.AssignedCustomer ?? "").ToLower().Contains(query)) ||
                ((SearchType == "الكل" || SearchType == "تفاصيل إضافية") && (g.AdditionalDetails ?? "").ToLower().Contains(query))
            );

            // تطبيق فلتر الشركة
            if (SelectedProvider != "الكل")
            {
                TelecomProvider provider = SelectedProvider switch
                {
                    "فودافون" => TelecomProvider.Vodafone,
                    "اتصالات" => TelecomProvider.Etisalat,
                    "وي" => TelecomProvider.We,
                    "أورانج" => TelecomProvider.Orange,
                    _ => TelecomProvider.Vodafone
                };
                groupsQuery = groupsQuery.Where(g => g.Provider == provider);
            }

            // تطبيق فلتر حالة المجموعة
            if (SelectedStatus != "الكل")
            {
                GroupStatus status = SelectedStatus switch
                {
                    "نشط" => GroupStatus.Active,
                    "موقوف" => GroupStatus.Suspended,
                    "محظور" => GroupStatus.Barred,
                    "بدون محفظة" => GroupStatus.Cashless,
                    "بمحفظة" => GroupStatus.CashWallet,
                    _ => GroupStatus.Active
                };
                groupsQuery = groupsQuery.Where(g => g.Status == status);
            }

            // تطبيق فلتر محفظة الكاش على المجموعات
            if (SelectedCashWallet == "نعم")
            {
                groupsQuery = groupsQuery.Where(g => g.RequiresCashWallet);
            }
            else if (SelectedCashWallet == "لا")
            {
                groupsQuery = groupsQuery.Where(g => !g.RequiresCashWallet);
            }

            var groups = groupsQuery.ToList();

            foreach (var group in groups)
            {
                // إضافة المجموعة نفسها
                SearchResults.Add(new SearchResult
                {
                    Type = "مجموعة",
                    GroupName = group.Name,
                    Provider = group.Provider.GetArabicName(),
                    ProviderColor = group.Provider.GetColorHex(),
                    LineCount = group.GetLineCount,
                    Details = $"عدد الخطوط: {group.GetLineCount}"
                });

                // إضافة خطوط المجموعة مع تطبيق جميع الفلاتر
                foreach (var line in group.Lines)
                {
                    // تطبيق فلتر البحث النصي على خطوط المجموعة
                    bool matchesTextSearch = 
                        ((SearchType == "الكل" || SearchType == "رقم قومي") && line.NationalId.ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "اسم الشخص") && line.Name.ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "رقم الخط") && line.PhoneNumber.ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "رقم داخلي") && line.InternalId.ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "محفظة كاش") && (line.CashWalletNumber ?? "").ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "نظام الخط") && (line.LineSystem ?? "").ToLower().Contains(query)) ||
                        ((SearchType == "الكل" || SearchType == "التفاصيل") && (line.Details ?? "").ToLower().Contains(query));
                    
                    if (!matchesTextSearch)
                        continue;
                    
                    // تطبيق فلتر محفظة الكاش على خطوط المجموعة أيضاً
                    if (SelectedCashWallet == "نعم" && !line.HasCashWallet)
                        continue;
                    if (SelectedCashWallet == "لا" && line.HasCashWallet)
                        continue;
                    
                    SearchResults.Add(new SearchResult
                    {
                        Type = "خط (من المجموعة)",
                        Name = line.Name,
                        PhoneNumber = line.PhoneNumber,
                        NationalId = line.NationalId,
                        InternalId = line.InternalId,
                        GroupName = group.Name,
                        Provider = group.Provider.GetArabicName(),
                        ProviderColor = group.Provider.GetColorHex(),
                        CashWalletNumber = line.CashWalletNumber,
                        HasCashWallet = line.HasCashWallet,
                        Details = line.Details
                    });
                }
            }
        }
    }
}

public class SearchResult
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string InternalId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ProviderColor { get; set; } = string.Empty;
    public string? CashWalletNumber { get; set; }
    public bool HasCashWallet { get; set; }
    public string? Details { get; set; }
    public int LineCount { get; set; }
}
