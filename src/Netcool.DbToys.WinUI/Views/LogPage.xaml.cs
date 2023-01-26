using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.ViewModels;

namespace Netcool.DbToys.Views;

public sealed partial class LogPage : Page
{
    public LogViewModel ViewModel { get; }

    public LogPage()
    {
        ViewModel = App.GetService<LogViewModel>();
        InitializeComponent();
    }

}