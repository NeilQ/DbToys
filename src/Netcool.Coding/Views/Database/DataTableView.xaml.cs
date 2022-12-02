using System.Reactive.Disposables;
using System.Windows.Controls;
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
        ViewModel = new DataTableViewModel();
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Columns, v => v.ColumnsGrid.ItemsSource).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.ResultSet.DefaultView, v => v.ResultSetGrid.ItemsSource).DisposeWith(d);
        });
    }

    private void ResultSetGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var header = e.Column.Header.ToString();

        // Replace all underscores with two underscores, to prevent AccessKey handling
        e.Column.Header = header?.Replace("_", "__");
    }
}