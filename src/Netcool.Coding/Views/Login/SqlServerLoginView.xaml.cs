using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Netcool.Coding.ViewModels;
using ReactiveUI;

namespace Netcool.Coding.Views
{
    /// <summary>
    /// Interaction logic for SqlServerLoginView.xaml
    /// </summary>
    public partial class SqlServerLoginView
    {
        public SqlServerLoginView()
        {
            InitializeComponent();
            ViewModel = new SqlServerLoginViewModel { CloseAction = Close };
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Server, v => v.Server.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Username, v => v.Username.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsSqlServerAuthentication, v => v.Username.IsEnabled);
                this.OneWayBind(ViewModel, vm => vm.IsSqlServerAuthentication, v => v.Password.IsEnabled);
                this.OneWayBind(ViewModel, vm => vm.AuthenticationTypes, v => v.AuthenticationBox.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsSqlServerAuthentication, v => v.AuthenticationBox.SelectedValue).DisposeWith(d);
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
