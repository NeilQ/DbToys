using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.ViewModels;

namespace Netcool.DbToys.Views;

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
