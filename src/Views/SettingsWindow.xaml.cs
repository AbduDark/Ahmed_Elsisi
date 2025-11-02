
using System.Windows;
using LineManagementSystem.Services;

namespace LineManagementSystem.Views;

public partial class SettingsWindow : Window
{
    public bool IsDarkMode
    {
        get => ThemeManager.IsDarkMode;
        set
        {
            ThemeManager.SetTheme(value);
            UpdateThemeText();
        }
    }

    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = this;
        UpdateThemeText();
    }

    private void UpdateThemeText()
    {
        if (CurrentThemeText != null)
        {
            var themeMode = ThemeManager.IsDarkMode ? "الوضع الليلي نشط" : "الوضع النهاري نشط";
            CurrentThemeText.Text = $"الوضع الحالي: {themeMode}";
        }
    }

    private void DarkModeToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (DarkModeToggle != null)
        {
            ThemeManager.SetTheme(DarkModeToggle.IsChecked == true);
            UpdateThemeText();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
