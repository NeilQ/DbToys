using Microsoft.UI.Xaml.Controls;

using Netcool.Coding.WinUI.ViewModels;

namespace Netcool.Coding.WinUI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
