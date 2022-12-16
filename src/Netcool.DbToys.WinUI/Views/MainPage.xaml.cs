using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.ViewModels;

namespace Netcool.DbToys.WinUI.Views;

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
