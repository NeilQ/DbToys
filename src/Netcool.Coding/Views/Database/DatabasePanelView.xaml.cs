using System.Reactive.Disposables;
using Netcool.Coding.ViewModels;
using ReactiveUI;

namespace Netcool.Coding.Views.Database;

/// <summary>
/// Interaction logic for DatabasePanelView.xaml
/// </summary>
public partial class DatabasePanelView
{
    public DatabasePanelView()
    {
        InitializeComponent();
        ViewModel = new DatabasePanelViewModel();
        ConnectionTree.ItemsSource = ViewModel.ConnectionItems;
        this.WhenActivated(d =>
        {
            this.WhenAnyValue(x => x.ConnectionTree.SelectedItem).BindTo(this, x => x.ViewModel.SelectedItem).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.PostgreSql).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.SqlServer).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.NewConnectionCommand, v => v.MySql).DisposeWith(d);
        });
    }
}