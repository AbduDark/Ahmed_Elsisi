using System.Windows;
using LineManagementSystem.Models;

namespace LineManagementSystem.Views;

public partial class GroupDialog : Window
{
    private readonly TelecomProvider _provider;
    private readonly LineGroup? _existingGroup;

    public LineGroup? ResultGroup { get; private set; }

    public GroupDialog(LineGroup? existingGroup, TelecomProvider provider)
    {
        InitializeComponent();

        _provider = provider;
        _existingGroup = existingGroup;

        cmbStatus.ItemsSource = Enum.GetValues(typeof(GroupStatus));
        cmbStatus.SelectedIndex = 0;

        if (existingGroup != null)
        {
            Title = "تعديل مجموعة";
            txtGroupName.Text = existingGroup.Name;
            cmbStatus.SelectedItem = existingGroup.Status;
            chkRequiresCashWallet.IsChecked = existingGroup.RequiresCashWallet;
            
            if (existingGroup.LastRenewedOn.HasValue)
                dtpLastRenewed.SelectedDate = existingGroup.LastRenewedOn.Value;
            
            txtEmployee.Text = existingGroup.AssignedToEmployee ?? string.Empty;
            txtCustomer.Text = existingGroup.AssignedCustomer ?? string.Empty;
            
            if (existingGroup.ExpectedHandoverDate.HasValue)
                dtpHandoverDate.SelectedDate = existingGroup.ExpectedHandoverDate.Value;
            
            chkIsHandedOver.IsChecked = existingGroup.IsHandedOver;
        }

        UpdateRenewalFieldsVisibility();
    }

    private void ChkRequiresCashWallet_Changed(object sender, RoutedEventArgs e)
    {
        UpdateRenewalFieldsVisibility();
    }

    private void UpdateRenewalFieldsVisibility()
    {
        var isVisible = chkRequiresCashWallet.IsChecked == true;
        lblLastRenewed.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        dtpLastRenewed.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtGroupName.Text))
        {
            MessageBox.Show("من فضلك أدخل اسم المجموعة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var group = _existingGroup ?? new LineGroup { Provider = _provider };
        
        group.Name = txtGroupName.Text.Trim();
        group.Status = (GroupStatus)cmbStatus.SelectedItem;
        group.RequiresCashWallet = chkRequiresCashWallet.IsChecked == true;
        
        if (group.RequiresCashWallet && dtpLastRenewed.SelectedDate.HasValue)
        {
            group.LastRenewedOn = dtpLastRenewed.SelectedDate.Value;
            group.NextRenewalDue = group.LastRenewedOn.Value.AddDays(60);
        }
        else
        {
            group.LastRenewedOn = null;
            group.NextRenewalDue = null;
        }

        group.AssignedToEmployee = string.IsNullOrWhiteSpace(txtEmployee.Text) ? null : txtEmployee.Text.Trim();
        group.AssignedCustomer = string.IsNullOrWhiteSpace(txtCustomer.Text) ? null : txtCustomer.Text.Trim();
        group.ExpectedHandoverDate = dtpHandoverDate.SelectedDate;
        group.IsHandedOver = chkIsHandedOver.IsChecked == true;

        if (group.IsHandedOver && !group.ActualHandoverDate.HasValue)
        {
            group.ActualHandoverDate = DateTime.Now;
        }

        ResultGroup = group;
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
