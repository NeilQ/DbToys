using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
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