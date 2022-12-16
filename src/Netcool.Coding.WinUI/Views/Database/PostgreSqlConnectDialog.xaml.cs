using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Netcool.Coding.WinUI.ViewModels.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Netcool.Coding.WinUI.Views.Database;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PostgreSqlConnectDialog : ContentDialog
{
    public PostgreSqlConnectViewModel ViewModel { get; }

    public PostgreSqlConnectDialog()
    {
        ViewModel=App.GetService<PostgreSqlConnectViewModel>();
        ViewModel.HasError = false;
        ViewModel.SchemaReader = null;
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Password = ((PasswordBox)sender).Password;
    }
}