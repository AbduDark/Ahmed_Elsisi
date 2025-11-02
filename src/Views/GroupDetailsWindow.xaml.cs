using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;
using Microsoft.Win32;

namespace LineManagementSystem.Views;

public partial class GroupDetailsWindow : Window
{
    private readonly GroupDetailsViewModel _viewModel;
    private readonly ProviderGroupsViewModel _parentViewModel;
    private readonly AlertService _alertService;
    private readonly ExportService _exportService;
    private readonly DatabaseContext _context;

    public GroupDetailsWindow(LineGroup group, ProviderGroupsViewModel parentViewModel, AlertService alertService)
    {
        InitializeComponent();

        _context = new DatabaseContext();
        // نتأكد إن الـ context نظيف من أي tracking قديم
        _context.ChangeTracker.Clear();

        var groupService = new GroupService(_context);
        _viewModel = new GroupDetailsViewModel(groupService, group);
        _parentViewModel = parentViewModel;
        _alertService = alertService;
        _exportService = new ExportService(_context);

        _viewModel.LineAdded += () =>
        {
            _parentViewModel.LoadGroups();
            txtPhoneNumber.Focus();
        };

        DataContext = _viewModel;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (_viewModel.IsAddingLine)
        {
            txtPhoneNumber.Focus();
        }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var textBox = sender as TextBox;
            if (textBox == txtName)
            {
                txtNationalId.Focus();
            }
            else if (textBox == txtNationalId)
            {
                txtPhoneNumber.Focus();
            }
            else if (textBox == txtPhoneNumber)
            {
                txtInternalId.Focus();
            }
            else if (textBox == txtInternalId)
            {
                if (_viewModel.SaveLineCommand.CanExecute(null))
                {
                    _viewModel.SaveLineCommand.Execute(null);
                }
            }
            else if (textBox == txtCashWalletNumber)
            {
                txtName.Focus();
            }
        }
    }

    private void BtnConfirm1_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 1;
        }
    }

    private void BtnConfirm2_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 2;
        }
    }

    private void BtnConfirm3_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 3;
        }
    }

    private void BtnConfirmClear_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 0;
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && !_viewModel.IsAddingLine)
        {
            if (_viewModel.AddLineCommand.CanExecute(null))
            {
                _viewModel.AddLineCommand.Execute(null);

                // التركيز على خانة رقم الخط بعد فتح النموذج
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtPhoneNumber.Focus();
                    txtPhoneNumber.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Placeholder for back button functionality if needed
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _alertService.CheckForAlerts();
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedLine != null)
        {
            if (_viewModel.EditLineCommand.CanExecute(null))
            {
                _viewModel.EditLineCommand.Execute(null);
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
                FileName = $"خطوط_{_viewModel.Group.Name}_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = _viewModel.Lines.ToList();
                _exportService.ExportLinesToExcel(saveDialog.FileName, lines);
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