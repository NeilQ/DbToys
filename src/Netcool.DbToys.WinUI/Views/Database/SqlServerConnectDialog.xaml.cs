using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.ViewModels.Database;

namespace Netcool.DbToys.WinUI.Views.Database;

public sealed partial class SqlServerConnectDialog
{
    public SqlServerConnectViewModel ViewModel { get; }
    public SqlServerConnectDialog()
    {
        ViewModel = App.GetService<SqlServerConnectViewModel>();
        ViewModel.IsActive = true;
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Password = ((PasswordBox)sender).Password;
    }

    private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        ViewModel.IsActive = false;
    }
}