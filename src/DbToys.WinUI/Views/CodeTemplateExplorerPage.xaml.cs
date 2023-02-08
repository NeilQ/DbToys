using Windows.Storage;
using Windows.UI.Core;
using DbToys.Helpers;
using DbToys.Services;
using DbToys.ViewModels;
using DbToys.ViewModels.CodeTemplate;
using DbToys.Views.CodeTemplate;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace DbToys.Views;

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
        ViewModel.ProjectCreatedAction = OnProjectCreated;
        InitializeComponent();
        LoadProjectTree();
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
        ((args.Tab.Content as Frame)!.Content as TemplatePage)?.CloseWebView2();
        sender.TabItems.Remove(args.Tab);
    }

    private TabViewItem CreateNewTab(TemplateFileItem item)
    {
        var newItem = new TabViewItem
        {
            Header = item.TabDisplayName,
            IconSource = new SymbolIconSource { Symbol = Symbol.Document }
        };
        var frame = new Frame();

        frame.Navigate(typeof(TemplatePage));
        (frame.GetPageViewModel() as TemplateViewModel)!.File = item.File;
        newItem.Content = frame;

        /*
        var page = App.GetService<TemplatePage>();
        page.ViewModel.File = item.File;
        page.Margin = new Thickness(0, 0, 0, 12);
        newItem.Content = page;
        */

        return newItem;
    }

    private async void LoadProjectTree()
    {
        ViewModel.TreeItems.Clear();
        var folders = await _templateStorageService.GetProjectFoldersAsync();
        foreach (var folder in folders)
        {
            var projectItem = new ProjectFolderItem(folder, false)
            {
                RenamedAction = OnProjectFolderRenamed,
                TemplateCreatedAction = OnTemplateFileCreated,
                DeletedAction = OnProjectFolderDeleted
            };
            LoadTemplateTree(projectItem);
            ViewModel.TreeItems.Add(projectItem);
        }
    }

    public async void LoadTemplateTree(ProjectFolderItem projectItem)
    {
        var files = await _templateStorageService.GetTemplateFilesAsync(projectItem.Folder);
        foreach (var file in files)
        {
            AddTemplateFileItem(projectItem, file);
        }
    }

    private void AddTemplateFileItem(ProjectFolderItem projectItem, StorageFile file)
    {
        var templateItem = new TemplateFileItem(file, projectItem.Folder)
        {
            RenamedAction = OnTemplateFileRenamed,
            DeletedAction = OnTemplateFileDeleted
        };
        projectItem.AddChild(templateItem);
    }

    private void OnProjectCreated(ProjectCreatedArg args)
    {
        var vm = new ProjectFolderItem(args.Folder, false)
        {
            RenamedAction = OnProjectFolderRenamed,
            TemplateCreatedAction = OnTemplateFileCreated,
            DeletedAction = OnProjectFolderDeleted
        };
        ViewModel.TreeItems.Add(vm);
    }

    private void OnTemplateFileCreated(TemplateCreatedArg args)
    {
        var folderName = args.File.Path.Split('\\')[^2];
        var projectItem = ViewModel.TreeItems.FirstOrDefault(t => t.Name.ToLower() == folderName.ToLower());
        if (projectItem == null) return;

        AddTemplateFileItem(projectItem, args.File);
        projectItem.IsExpanded = true;
    }

    private async void OnTemplateFileRenamed(RenamedArgs args)
    {
        var folderName = args.OldPath.Split('\\')[^2];
        var projectItem = ViewModel.TreeItems.FirstOrDefault(t => t.Name.ToLower() == folderName.ToLower());

        // update file item
        var fileItem = projectItem?.Children.FirstOrDefault(t => t.Name.ToLower() == args.OldName.ToLower()) as TemplateFileItem;
        if (fileItem == null) return;
        fileItem.Name = args.NewName;
        fileItem.File = await StorageFile.GetFileFromPathAsync(args.NewPath);

        // update tab view item
        var tabItem = TemplateTabView.TabItems
            .FirstOrDefault(t => (string)(t as TabViewItem)!.Tag == args.OldPath) as TabViewItem;
        if (tabItem == null) return;
        tabItem.Tag = args.NewPath;
        tabItem.Header = fileItem.TabDisplayName;
        var frame = tabItem.Content as Frame;
        (frame.GetPageViewModel() as TemplateViewModel)!.File  = fileItem.File;
    }

    private void OnTemplateFileDeleted(TemplateDeletedArg args)
    {
        var projectItem = ViewModel.TreeItems.FirstOrDefault(t => t.Name.ToLower() == args.FolderName.ToLower());
        if (projectItem == null) return;

        // remove file tree item
        var fileItem = projectItem.Children.FirstOrDefault(t => t.Name.ToLower() == args.FileName.ToLower());
        if (fileItem == null) return;
        projectItem.Children.Remove(fileItem);

        // remove open tab view item
        var tabItem = TemplateTabView.TabItems
            .FirstOrDefault(t => (string)(t as TabViewItem)!.Tag == args.FilePath);
        if (tabItem != null)
        {
            TemplateTabView.TabItems.Remove(tabItem);
        }
    }

    private async void OnProjectFolderRenamed(RenamedArgs args)
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

    private void OnProjectFolderDeleted(ProjectDeletedArg args)
    {
        var projectItem = ViewModel.TreeItems.FirstOrDefault(t => t.Name.ToLower() == args.FolderName.ToLower());
        if (projectItem == null) return;

        ViewModel.TreeItems.Remove(projectItem);

        // remove tab items
        var tabItems = TemplateTabView.TabItems
            .Where(t => ((string)(t as TabViewItem)!.Tag).StartsWith(args.FolderPath));
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