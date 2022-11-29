using System.Reactive.Disposables;
using Netcool.Coding.ViewModels.Database;
using ReactiveUI;

namespace Netcool.Coding.Views.Database;

/// <summary>
/// Interaction logic for DataTableView.xaml
/// </summary>
public partial class DataTableView
{
    public DataTableView()
    {
        InitializeComponent();
        this.ViewModel = new DataTableViewModel();
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Columns, v => v.ColumnsGrid.ItemsSource).DisposeWith(d);
        });
    }
}