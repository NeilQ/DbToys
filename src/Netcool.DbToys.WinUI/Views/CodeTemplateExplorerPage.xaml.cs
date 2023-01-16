using Windows.Storage;
using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.Services;
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

    private readonly CodeTemplateStorageService _templateStorageService;

    public CodeTemplateExplorerViewModel ViewModel { get; }

    public CodeTemplateExplorerPage()
    {
        ViewModel = App.GetService<CodeTemplateExplorerViewModel>();
        _templateStorageService = App.GetService<CodeTemplateStorageService>();
        ViewModel.ReloadAction = LoadProjectTree;
        LoadProjectTree();
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
                var view = CreateNewTab(fileItem);
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

    private TabViewItem CreateNewTab(TemplateFileItem item)
    {
        var newItem = new TabViewItem
        {
            Header = item.TabDisplayName,
            IconSource = new SymbolIconSource { Symbol = Symbol.Document }
        };
        var page = App.GetService<TemplatePage>();
        page.ViewModel.LoadFile(item.File);
        page.Margin = new Thickness(0, 0, 0, 12);
        newItem.Content = page;

        return newItem;
    }

    private async void LoadProjectTree()
    {
        ViewModel.TreeItems.Clear();
        var folders = await _templateStorageService.GetProjectFoldersAsync();
        foreach (var folder in folders)
        {
            var projectItem = new ProjectFolderItem(folder, false);
            projectItem.RenamedAction = Renamed;
            var files = await _templateStorageService.GetTemplateFiles(folder);
            foreach (var file in files)
            {
                var templateItem = new TemplateFileItem(file, folder);
                projectItem.AddChild(templateItem);
            }
            ViewModel.TreeItems.Add(projectItem);
        }
    }

    public async void LoadTemplateTree(ProjectFolderItem projectItem)
    {
        var files = await _templateStorageService.GetTemplateFiles(projectItem.Folder);
        foreach (var file in files)
        {
            var templateItem = new TemplateFileItem(file, projectItem.Folder);
            projectItem.AddChild(templateItem);
        }
    }

    private async void Renamed(RenamedArgs args)
    {
        var projectItem = ViewModel.TreeItems.FirstOrDefault(t => t.Name == args.OldName);
        if (projectItem == null) return;
        projectItem.Name = args.NewName;
        projectItem.Folder = await StorageFolder.GetFolderFromPathAsync(args.NewPath);

        // reload files
        projectItem.Children.Clear();
        LoadTemplateTree(projectItem);

        // remove tab items
        var tabItems = TemplateTabView.TabItems
            .Where(t => ((string)(t as TabViewItem)!.Tag).StartsWith(args.OldPath));
        if (!tabItems.Any()) return;
        foreach (var tabItem in tabItems)
        {
            TemplateTabView.TabItems.Remove(tabItem);
        }
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