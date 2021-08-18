using System.Reactive.Disposables;
using System.Windows;
using Netcool.Workshop.ViewModels;
using ReactiveUI;

namespace Netcool.Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<MainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty;
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
        }

        static MainWindow()
        {
            ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(MainWindowViewModel), typeof(MainWindow));
        }

        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }
    }
}
