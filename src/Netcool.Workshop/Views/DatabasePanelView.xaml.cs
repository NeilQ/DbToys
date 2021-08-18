using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Netcool.Workshop.ViewModels;
using ReactiveUI;

namespace Netcool.Workshop.Views
{
    /// <summary>
    /// Interaction logic for DatabasePanelView.xaml
    /// </summary>
    public partial class DatabasePanelView
    {
        public DatabasePanelView()
        {
            InitializeComponent();
            ViewModel = new DatabasePanelViewModel();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ConnectionItems, v => v.ConnectionTree.ItemsSource).DisposeWith(d);
                this.WhenAnyValue(x => x.ConnectionTree.SelectedItem).BindTo(this, x => x.ViewModel.SelectedItem);
                this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.PostgreSql).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.SqlServer).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.MySql).DisposeWith(d);
            });
        }
    }
}
