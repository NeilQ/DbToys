using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.Helpers;
using Netcool.DbToys.Services;
using Netcool.DbToys.ViewModels.CodeTemplate;

namespace Netcool.DbToys.ViewModels;

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

    public IRelayCommand ReloadCommand { get; set; }
    public IAsyncRelayCommand CreateProjectCommand { get; set; }
    public IRelayCommand ExplorerCommand { get; set; }

    public Action ReloadAction { get; set; }

    public CodeTemplateExplorerViewModel(INotificationService notificationService, CodeTemplateStorageService templateStorageService)
    {
        _notificationService = notificationService;
        _templateStorageService = templateStorageService;
        CreateProjectCommand = new AsyncRelayCommand(CreateProjectAsync);
        ReloadCommand = new RelayCommand(ReloadProjectTree);
        ExplorerCommand = new RelayCommand(ShowInExplorer);
    }

    private void ShowInExplorer()
    {
        try
        {
            Process.Start("explorer.exe", _templateStorageService.TemplateFolderPath);
        }
        catch (Win32Exception win32Exception)
        {
            _notificationService.Error(win32Exception.Message);
        }
    }

    private async Task CreateProjectAsync()
    {
        var dialog = DialogFactory.GetFor_CreateProjectDialog();
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
            _notificationService.Error($"Create project folder failed with error: {ex.Message}");
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