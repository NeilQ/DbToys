using Windows.UI.Core;
using CommunityToolkit.WinUI.UI.Controls;
using DbToys.Helpers;
using DbToys.ViewModels;
using DbToys.ViewModels.Database;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;

namespace DbToys.Views;

public sealed partial class DatabasePage
{
    private readonly InputCursor _arrowCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 0));

    private readonly InputCursor _resizeCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.SizeWestEast, 1));

    public DatabaseViewModel ViewModel { get; }

    public DatabasePage()
    {
        ViewModel = App.GetService<DatabaseViewModel>();
        InitializeComponent();
        ViewModel.OnResultSetLoaded += columns =>
        {
            ResultSetGrid.Columns.Clear();
            if (columns == null) return;
            for (var i = 0; i < columns.Count; i++)
            {
                ResultSetGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }
        };
    }

    private void GridSplitter_OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        this.ChangeCursor(_resizeCursor);
    }

    private void GridSplitter_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        this.ChangeCursor(_arrowCursor);
    }

    private void GridSplitter_OnLoaded(object sender, RoutedEventArgs e)
    {
        (sender as UIElement)?.ChangeCursor(_resizeCursor);
    }

    private void TreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is DatabaseItem item)
        {
            item.IsExpanded = !item.IsExpanded;
        }
    }
}

public class DatabaseTreeItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ConnectionTemplate { get; set; }
    public DataTemplate DatabaseTemplate { get; set; }
    public DataTemplate TableTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is ConnectionItem) return ConnectionTemplate;
        if (item is DatabaseItem) return DatabaseTemplate;
        if (item is TableItem) return TableTemplate;
        return ConnectionTemplate;
    }
}
