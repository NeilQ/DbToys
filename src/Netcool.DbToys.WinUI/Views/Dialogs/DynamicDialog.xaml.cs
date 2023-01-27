using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.Helpers;
using Netcool.DbToys.ViewModels.Dialogs;

namespace Netcool.DbToys.Views.Dialogs;

public sealed partial class DynamicDialog
{
    public DynamicDialog(DynamicDialogViewModel vm)
    {
        ViewModel = vm;
        InitializeComponent();
        ViewModel.HideDialog = Hide;
    }

    public DynamicDialogViewModel ViewModel { get; set; }

    public new Task<ContentDialogResult> ShowAsync() => DialogHelper.SetContentDialogRoot(this).ShowAsync().AsTask();

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.PrimaryButtonCommand.Execute(args);
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.SecondaryButtonCommand.Execute(args);
    }

    private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.CloseButtonCommand.Execute(args);
    }

    private void ContentDialog_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        ViewModel.KeyDownCommand.Execute(e);
    }
}