using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.Services;

namespace Netcool.DbToys.WinUI.ViewModels.Dialogs;

public class GenerateCodeViewModel : ObservableRecipient
{
    private readonly CodeTemplateStorageService _storageService;

    private StorageFolder _selectedFolder;
    public StorageFolder SelectedFolder
    {
        get => _selectedFolder;
        set => SetProperty(ref _selectedFolder, value);
    }

    private string _outputPath;
    public string OutputPath { get => _outputPath; set => SetProperty(ref _outputPath, value); }

    public StorageFolder OutputFolder { get; private set; }

    public IAsyncRelayCommand PickOutputFolderCommand { get; set; }

    public ObservableCollection<StorageFolder> ProjectFolders { get; set; } = new();

    public GenerateCodeViewModel(CodeTemplateStorageService storageService)
    {
        _storageService = storageService;
        PickOutputFolderCommand = new AsyncRelayCommand(PickOutputFolder);
    }

    private async Task PickOutputFolder()
    {
        var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.Desktop };

        // When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
        if (Window.Current == null)
        {
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, Win32Helper.GetActiveWindow());
        }

        OutputFolder = await folderPicker.PickSingleFolderAsync();
        if (OutputFolder != null)
        {
            OutputPath = OutputFolder.Path;
        }
    }

    protected override async void OnActivated()
    {
        var folders = await _storageService.GetProjectFoldersAsync();
        if (folders is { Count: > 0 })
        {
            foreach (var storageFolder in folders)
            {
                ProjectFolders.Add(storageFolder);
            }
        }
    }
}