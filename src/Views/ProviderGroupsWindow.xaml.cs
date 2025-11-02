using System.Windows;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;
using Microsoft.Win32;

namespace LineManagementSystem.Views;

public partial class ProviderGroupsWindow : Window
{
    private readonly ProviderGroupsViewModel _viewModel;
    private readonly AlertService _alertService;
    private readonly ExportService _exportService;

    public ProviderGroupsWindow(TelecomProvider provider, GroupService groupService, AlertService alertService)
    {
        InitializeComponent();

        _alertService = alertService;
        _viewModel = new ProviderGroupsViewModel(groupService, provider);
        _viewModel.NavigateToGroupDetails += OnNavigateToGroupDetails;
        _viewModel.OpenGroupDialog += OnOpenGroupDialog;

        var context = new DatabaseContext();
        _exportService = new ExportService(context);

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

    private void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.SelectedGroup != null)
        {
            if (_viewModel.ViewGroupDetailsCommand.CanExecute(null))
            {
                _viewModel.ViewGroupDetailsCommand.Execute(null);
            }
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedGroup != null)
        {
            if (_viewModel.EditGroupCommand.CanExecute(null))
            {
                _viewModel.EditGroupCommand.Execute(null);
            }
        }
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"مجموعات_{_viewModel.Provider}_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var groups = _viewModel.Groups.ToList();
                _exportService.ExportGroupsToExcel(saveDialog.FileName, groups);
                MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"تقرير_{DateTime.Now:yyyy-MM-dd}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                _exportService.ExportToPdf(saveDialog.FileName);
                MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}