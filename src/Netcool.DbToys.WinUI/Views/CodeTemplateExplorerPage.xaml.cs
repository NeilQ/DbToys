using Windows.Storage;
using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.ViewModels;
using Netcool.DbToys.WinUI.ViewModels.CodeTemplate;
using Netcool.DbToys.WinUI.Views.CodeTemplate;

namespace Netcool.DbToys.WinUI.Views;

public sealed partial class CodeTemplateExplorerPage : Page
{
    private readonly InputCursor _arrowCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 0));

    private readonly InputCursor _resizeCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.SizeWestEast, 1));

    private const int MaxTabCapacity = 10;

    public CodeTemplateExplorerViewModel ViewModel { get; }

    public CodeTemplateExplorerPage()
    {
        ViewModel = App.GetService<CodeTemplateExplorerViewModel>();
        ViewModel.IsActive = true;
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

    private void TreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is ProjectFolderItem projectItem)
        {
            projectItem.IsExpanded = !projectItem.IsExpanded;
        }
        else if (args.InvokedItem is TemplateFileItem fileItem)
        {
            var openedView =
                TemplateTabView.TabItems.FirstOrDefault(t => (string)(t as TabViewItem)!.Tag! == fileItem!.File!.Path);
            if (openedView != null)
            {
                TemplateTabView.SelectedItem = openedView;
            }
            else
            {
                var view = CreateNewTab(fileItem.File);
                view.Tag = fileItem.File.Path;
                TemplateTabView.TabItems.Add(view);
                TemplateTabView.SelectedItem = view;

                if (TemplateTabView.TabItems.Count > MaxTabCapacity)
                {
                    TemplateTabView.TabItems.RemoveAt(0);
                }
            }
        }
    }

    private void TabView_OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }

    private TabViewItem CreateNewTab(StorageFile file)
    {
        var newItem = new TabViewItem
        {
            Header = file.Name,
            IconSource = new SymbolIconSource { Symbol = Symbol.Document },
        };
        var page = App.GetService<TemplatePage>();
        page.ViewModel.LoadFile(file);
        page.Margin = new Thickness(0, 0, 0, 12);
        newItem.Content = page;

        return newItem;
    }

}

public class CodeTemplateTreeItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ProjectFolderTemplate { get; set; }
    public DataTemplate TemplateFileTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is ProjectFolderItem) return ProjectFolderTemplate;
        if (item is TemplateFileItem) return TemplateFileTemplate;
        return ProjectFolderTemplate;
    }
}