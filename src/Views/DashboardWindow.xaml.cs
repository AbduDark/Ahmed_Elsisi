
using System.Windows;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class DashboardWindow : Window
{
    public DashboardWindow(DatabaseContext dbContext, AlertService alertService)
    {
        InitializeComponent();
        DataContext = new DashboardViewModel(dbContext, alertService);
    }
}
