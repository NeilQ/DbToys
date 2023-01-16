using System.Collections.ObjectModel;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.Helpers;
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

    public ObservableCollection<ProjectFolderItem> TreeItems = new();

    private readonly INotificationService _notificationService;
    private readonly CodeTemplateStorageService _templateStorageService;

    public IAsyncRelayCommand CreateProjectCommand { get; set; }
    public IRelayCommand ReloadCommand { get; set; }

    public Action ReloadAction { get; set; }

    public CodeTemplateExplorerViewModel(INotificationService notificationService, CodeTemplateStorageService templateStorageService)
    {
        _notificationService = notificationService;
        _templateStorageService = templateStorageService;
        CreateProjectCommand = new AsyncRelayCommand(CreateProjectAsync);
        ReloadCommand = new RelayCommand(ReloadProjectTree);
        // rename project
        // todo: rename template
        // todo: delete project
        // todo: delete template
    }

    private async Task CreateProjectAsync()
    {
        var dialog = DynamicDialogFactory.GetFor_CreateProjectDialog();
        await dialog.ShowAsync();
        string folderName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            folderName = (string)dialog.ViewModel.AdditionalData;
        }
        else return;

        if (string.IsNullOrEmpty(folderName)) return;
        var path = Path.Join(_templateStorageService.TemplateFolderPath, folderName);
        if (Directory.Exists(path)) return;
        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            _notificationService.Error($"Create project folder failed with error: {ex.Message}",
                Constants.Notification.ShortErrorDuration);
            return;
        }
        var storage = await StorageFolder.GetFolderFromPathAsync(path);
        var vm = new ProjectFolderItem(storage, false);
        TreeItems.Add(vm);
    }

    public void ReloadProjectTree()
    {
        ReloadAction?.Invoke();
    }
}