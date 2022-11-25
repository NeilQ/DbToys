using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Netcool.Coding.ViewModels;
using ReactiveUI;

namespace Netcool.Coding.Views
{
    /// <summary>
    /// Interaction logic for PostgreSqlLoginView.xaml
    /// </summary>
    public partial class PostgreSqlLoginView
    {
        public PostgreSqlLoginView()
        {
            InitializeComponent();
            ViewModel = new PostgreSqlLoginViewModel {CloseAction = Close};
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.ServerIp, v => v.ServerIp.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Port, v => v.Port.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Username, v => v.Username.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Connect, v => v.ConnectButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton).DisposeWith(d);
            });
        }

        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
