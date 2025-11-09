
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LineManagementSystem.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DatabaseContext _dbContext;
    private readonly AlertService _alertService;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _totalGroups;
    public int TotalGroups
    {
        get => _totalGroups;
        set
        {
            _totalGroups = value;
            OnPropertyChanged();
        }
    }

    private int _totalLines;
    public int TotalLines
    {
        get => _totalLines;
        set
        {
            _totalLines = value;
            OnPropertyChanged();
        }
    }

    private int _activeGroups;
    public int ActiveGroups
    {
        get => _activeGroups;
        set
        {
            _activeGroups = value;
            OnPropertyChanged();
        }
    }

    private int _renewalAlerts;
    public int RenewalAlerts
    {
        get => _renewalAlerts;
        set
        {
            _renewalAlerts = value;
            OnPropertyChanged();
        }
    }

    private PlotModel? _providerDistributionPlot;
    public PlotModel? ProviderDistributionPlot
    {
        get => _providerDistributionPlot;
        set
        {
            _providerDistributionPlot = value;
            OnPropertyChanged();
        }
    }

    private PlotModel? _statusDistributionPlot;
    public PlotModel? StatusDistributionPlot
    {
        get => _statusDistributionPlot;
        set
        {
            _statusDistributionPlot = value;
            OnPropertyChanged();
        }
    }

    private PlotModel? _renewalTimelinePlot;
    public PlotModel? RenewalTimelinePlot
    {
        get => _renewalTimelinePlot;
        set
        {
            _renewalTimelinePlot = value;
            OnPropertyChanged();
        }
    }

    public DashboardViewModel(DatabaseContext dbContext, AlertService alertService)
    {
        _dbContext = dbContext;
        _alertService = alertService;
        LoadData();
    }

    private void LoadData()
    {
        TotalGroups = _dbContext.LineGroups.Count();
        TotalLines = _dbContext.PhoneLines.Count();
        ActiveGroups = _dbContext.LineGroups.Count(g => g.Status == GroupStatus.Active);
        RenewalAlerts = _alertService.GetActiveAlerts().Count;

        CreateProviderDistributionChart();
        CreateStatusDistributionChart();
        CreateRenewalTimelineChart();
    }

    private void CreateProviderDistributionChart()
    {
        var plotModel = new PlotModel { Title = "" };

        var pieSeries = new PieSeries
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.5,
            AngleSpan = 360,
            StartAngle = 0
        };

        var providerGroups = _dbContext.LineGroups
            .GroupBy(g => g.Provider)
            .Select(g => new { Provider = g.Key, Count = g.Count() })
            .ToList();

        foreach (var group in providerGroups)
        {
            var color = GetProviderColor(group.Provider);
            pieSeries.Slices.Add(new PieSlice(group.Provider.GetArabicName(), group.Count)
            {
                Fill = OxyColor.Parse(color)
            });
        }

        plotModel.Series.Add(pieSeries);
        ProviderDistributionPlot = plotModel;
    }

    private void CreateStatusDistributionChart()
    {
        var plotModel = new PlotModel { Title = "" };

        var pieSeries = new PieSeries
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.5,
            AngleSpan = 360,
            StartAngle = 0
        };

        var statusGroups = _dbContext.LineGroups
            .GroupBy(g => g.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToList();

        foreach (var group in statusGroups)
        {
            var color = GetStatusColor(group.Status);
            pieSeries.Slices.Add(new PieSlice(group.Status.GetArabicName(), group.Count)
            {
                Fill = OxyColor.Parse(color)
            });
        }

        plotModel.Series.Add(pieSeries);
        StatusDistributionPlot = plotModel;
    }

    private void CreateRenewalTimelineChart()
    {
        var plotModel = new PlotModel { Title = "" };

        var dateAxis = new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd/MM",
            Title = "التاريخ",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        };

        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "عدد المجموعات",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        };

        plotModel.Axes.Add(dateAxis);
        plotModel.Axes.Add(valueAxis);

        var today = DateTime.Today;
        var renewalGroups = _dbContext.LineGroups
            .Where(g => g.RenewalDate.HasValue && g.RenewalDate.Value <= today.AddDays(30))
            .OrderBy(g => g.RenewalDate)
            .ToList();

        var lineSeries = new LineSeries
        {
            Title = "مجموعات التجديد",
            MarkerType = MarkerType.Circle,
            Color = OxyColors.OrangeRed,
            MarkerSize = 5,
            MarkerStroke = OxyColors.White,
            MarkerStrokeThickness = 1.5
        };

        var groupedByDate = renewalGroups
            .GroupBy(g => g.RenewalDate!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(g => g.Date)
            .ToList();

        foreach (var point in groupedByDate)
        {
            lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Date), point.Count));
        }

        plotModel.Series.Add(lineSeries);
        RenewalTimelinePlot = plotModel;
    }

    private string GetProviderColor(TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => "#E60000",
            TelecomProvider.Etisalat => "#007E3A",
            TelecomProvider.We => "#60269E",
            TelecomProvider.Orange => "#FF7900",
            _ => "#000000"
        };
    }

    private string GetStatusColor(GroupStatus status)
    {
        return status switch
        {
            GroupStatus.Active => "#4CAF50",
            GroupStatus.Suspended => "#FFC107",
            GroupStatus.Barred => "#F44336",
            GroupStatus.Cashless => "#9E9E9E",
            GroupStatus.CashWallet => "#2196F3",
            _ => "#000000"
        };
    }
}
