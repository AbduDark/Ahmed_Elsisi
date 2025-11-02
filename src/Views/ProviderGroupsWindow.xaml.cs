using System.Windows;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class ProviderGroupsWindow : Window
{
    private readonly ProviderGroupsViewModel _viewModel;
    private readonly AlertService _alertService;

    public ProviderGroupsWindow(TelecomProvider provider, GroupService groupService, AlertService alertService)
    {
        InitializeComponent();

        _alertService = alertService;
        _viewModel = new ProviderGroupsViewModel(groupService, provider);
        _viewModel.NavigateToGroupDetails += OnNavigateToGroupDetails;
        _viewModel.OpenGroupDialog += OnOpenGroupDialog;

        DataContext = _viewModel;
    }

    private void OnNavigateToGroupDetails(LineGroup group)
    {
        var detailsWindow = new GroupDetailsWindow(group, _viewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnOpenGroupDialog(LineGroup? group)
    {
        var dialog = new GroupDialog(group, _viewModel.Provider);
        if (dialog.ShowDialog() == true && dialog.ResultGroup != null)
        {
            _viewModel.SaveGroup(dialog.ResultGroup);
        }
    }
}
