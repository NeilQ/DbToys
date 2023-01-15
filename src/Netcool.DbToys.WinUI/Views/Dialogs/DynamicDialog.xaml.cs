using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.ViewModels.Dialogs;

namespace Netcool.DbToys.WinUI.Views.Dialogs;

public sealed partial class DynamicDialog
{
    public DynamicDialog(DynamicDialogViewModel vm)
    {
        ViewModel = vm;
        InitializeComponent();
        ViewModel.HideDialog = Hide;
    }

    public DynamicDialogViewModel ViewModel { get; set; }

    public new Task<ContentDialogResult> ShowAsync() => SetContentDialogRoot(this).ShowAsync().AsTask();

    private ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
    {
        if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        {
            contentDialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        }
        return contentDialog;
    }

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