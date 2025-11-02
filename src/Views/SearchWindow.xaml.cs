using System.Windows;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;
using Microsoft.Win32;

namespace LineManagementSystem.Views;

public partial class SearchWindow : Window
{
    private readonly SearchViewModel _viewModel;
    private readonly GroupService _groupService;
    private readonly AlertService _alertService;
    private readonly ExportService _exportService;
    private readonly DatabaseContext _context;

    public SearchWindow(GroupService groupService, AlertService alertService)
    {
        InitializeComponent();

        _context = new DatabaseContext();
        _viewModel = new SearchViewModel(groupService, _context);
        _groupService = groupService;
        _alertService = alertService;
        _exportService = new ExportService(_context);

        _viewModel.NavigateToGroupDetails += OnNavigateToGroupDetails;
        _viewModel.EditLine += OnEditLine;
        _viewModel.DeleteLine += OnDeleteLine;
        _viewModel.DeleteGroup += OnDeleteGroup;

        DataContext = _viewModel;
    }

    private void OnNavigateToGroupDetails(LineGroup group)
    {
        var parentViewModel = new ProviderGroupsViewModel(_groupService, group.Provider);
        var detailsWindow = new GroupDetailsWindow(group, parentViewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnEditLine(PhoneLine line, LineGroup group)
    {
        var parentViewModel = new ProviderGroupsViewModel(_groupService, group.Provider);
        var detailsWindow = new GroupDetailsWindow(group, parentViewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnDeleteLine(int lineId)
    {
        try
        {
            _groupService.DeleteLine(lineId);
            System.Windows.MessageBox.Show("تم حذف الخط بنجاح", "نجح", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void OnDeleteGroup(int groupId)
    {
        try
        {
            _groupService.DeleteGroup(groupId);
            System.Windows.MessageBox.Show("تم حذف المجموعة بنجاح", "نجح", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.EditItemCommand.CanExecute(null))
        {
            var dataGrid = sender as System.Windows.Controls.DataGrid;
            if (dataGrid?.SelectedItem != null)
            {
                _viewModel.EditItemCommand.Execute(dataGrid.SelectedItem);
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"نتائج_البحث_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = _viewModel.SearchResults
                    .Where(r => r.Type == "خط")
                    .Select(r => _context.PhoneLines.FirstOrDefault(l => l.PhoneNumber == r.PhoneNumber))
                    .Where(l => l != null)
                    .ToList();

                if (lines.Any())
                {
                    _exportService.ExportLinesToExcel(saveDialog.FileName, lines);
                    MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("لا توجد خطوط للتصدير", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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