using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Netcool.DbToys.WinUI.Services;
using Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

namespace Netcool.DbToys.WinUI.ViewModels;

public class CodeTemplateExplorerViewModel : ObservableRecipient
{
    private object _selectedItem;
    public object SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ObservableCollection<TreeItem> TreeItems = new();

    private readonly INotificationService _notificationService;
    private readonly CodeTemplateStorageService _templateStorageService;

    public CodeTemplateExplorerViewModel(INotificationService notificationService, CodeTemplateStorageService templateStorageService)
    {
        _notificationService = notificationService;
        _templateStorageService = templateStorageService;
    }

    protected override async void OnActivated()
    {
        var folders = await _templateStorageService.GetProjectFoldersAsync();
        TreeItems.Clear();
        foreach (var folder in folders)
        {
            var projectItem = new ProjectFolderItem(folder, false);
            projectItem.LoadIcon();
            var files = await _templateStorageService.GetTemplateFiles(folder);
            foreach (var file in files)
            {
                var templateItem = new TemplateFileItem(file, false);
                templateItem.LoadIcon();
                projectItem.AddChild(templateItem);
            }
            TreeItems.Add(projectItem);
        }
    }
}