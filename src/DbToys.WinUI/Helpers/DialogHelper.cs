using Microsoft.UI.Xaml.Controls;

namespace DbToys.Helpers;

public class DialogHelper
{
    public static ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
    {
        contentDialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        return contentDialog;
    }
}