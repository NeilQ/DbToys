using CommunityToolkit.WinUI.UI.Controls;
using DbToys.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace DbToys.Views;

public sealed partial class TableDetailPage : Page
{
    public TableDetailViewModel ViewModel { get; }

    public TableDetailPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<TableDetailViewModel>();

        ViewModel.OnResultSetLoaded += columns =>
        {
            ResultSetGrid.Columns.Clear();
            if (columns == null) return;
            for (var i = 0; i < columns.Count; i++)
            {
                ResultSetGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i + "]") }
                });
            }
        };
    }
}