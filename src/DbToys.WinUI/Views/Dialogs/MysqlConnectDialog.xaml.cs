using DbToys.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.Views.Dialogs;

public sealed partial class MysqlConnectDialog
{
    public MysqlConnectViewModel ViewModel { get; }
    public MysqlConnectDialog()
    {
        ViewModel = App.GetService<MysqlConnectViewModel>();
        ViewModel.IsActive = true;
        ViewModel.PasswordChanged += s => { PasswordBox!.Password = s; };
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