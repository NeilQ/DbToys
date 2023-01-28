using DbToys.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.Views;

public sealed partial class LogPage : Page
{
    public LogViewModel ViewModel { get; }

    public LogPage()
    {
        ViewModel = App.GetService<LogViewModel>();
        InitializeComponent();
    }
}