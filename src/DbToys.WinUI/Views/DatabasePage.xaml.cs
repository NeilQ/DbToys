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
using DbToys.Views.CodeTemplate;

namespace DbToys.Views;

public sealed partial class DatabasePage
{
    private readonly InputCursor _arrowCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 0));

    private readonly InputCursor _resizeCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.SizeWestEast, 1));

    public DatabaseViewModel ViewModel { get; }

    private const int MaxTabCapacity = 6;

    public DatabasePage()
    {
        ViewModel = App.GetService<DatabaseViewModel>();
        InitializeComponent();
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

    private void TabView_OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        ((args.Tab.Content as Frame)!.Content as TableDetailPage)!.ViewModel.IsActive = false;
        sender.TabItems.Remove(args.Tab);
    }
    private TabViewItem CreateNewTab(TableItem item)
    {
        var newItem = new TabViewItem
        {
            Header = item.Table.DisplayName,
            IconSource = new SymbolIconSource { Symbol = Symbol.Document }
        };
        var frame = new Frame();

        frame.Navigate(typeof(TableDetailPage));

        var viewModel = frame.GetPageViewModel() as TableDetailViewModel;
        viewModel!.SchemaReader = ViewModel.SchemaReader;
        viewModel.SelectedTable = item.Table;
        viewModel.IsActive = true;
        newItem.Content = frame;

        return newItem;
    }

    private void TreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is DatabaseItem item)
        {
            item.IsExpanded = !item.IsExpanded;
        }
        else if (args.InvokedItem is TableItem tableItem)
        {
            var openedView = TableDetailTabView.TabItems.FirstOrDefault(t =>
                (string)(t as TabViewItem)!.Tag! == tableItem.Table.DisplayName);
            if (openedView != null)
            {
                TableDetailTabView.SelectedItem = openedView;
            }
            else
            {
                var view = CreateNewTab(tableItem);
                view.Tag = tableItem.Table.DisplayName;
                TableDetailTabView.TabItems.Add(view);
                TableDetailTabView.SelectedItem = view;

                if (TableDetailTabView.TabItems.Count > MaxTabCapacity)
                {

                    (((TableDetailTabView.TabItems[0] as TabViewItem)!.Content as Frame)!.Content as TableDetailPage)!
                        .ViewModel.IsActive = false;
                    TableDetailTabView.TabItems.RemoveAt(0);
                }
            }
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
