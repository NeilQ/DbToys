using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbToys.Core;
using DbToys.Helpers;
using DbToys.Services;
using DbToys.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.ViewModels.Dialogs;

public class GenerateCodeViewModel : ObservableRecipient
{
    private readonly CodeTemplateStorageService _storageService;
    private readonly UiSettingsService _uiSettingsService;

    public IRelayCommand ConfirmCommand { get; set; }

    private int _selectedProjectIndex;
    public int SelectedProjectIndex
    {
        get => _selectedProjectIndex;
        set => SetProperty(ref _selectedProjectIndex, value);
    }

    private StorageFolder _templateProjectFolder;
    public StorageFolder TemplateProjectFolder
    {
        get => _templateProjectFolder;
        set => SetProperty(ref _templateProjectFolder, value);
    }

    public ContentDialogResult DialogResult { get; private set; }

    private string _outputPath;
    public string OutputPath { get => _outputPath; set => SetProperty(ref _outputPath, value); }

    public IAsyncRelayCommand PickOutputFolderCommand { get; set; }

    public ObservableCollection<StorageFolder> ProjectFolders { get; set; } = new();

    public GenerateCodeViewModel(CodeTemplateStorageService storageService, UiSettingsService uiSettingsService)
    {
        _storageService = storageService;
        _uiSettingsService = uiSettingsService;
        PickOutputFolderCommand = new AsyncRelayCommand(PickOutputFolder);
        ConfirmCommand = new RelayCommand(OnConfirm);
    }

    private void OnConfirm()
    {
        if (_uiSettingsService.CodeGeneratorOutputPath != OutputPath && !string.IsNullOrEmpty(OutputPath))
            _uiSettingsService.CodeGeneratorOutputPath = OutputPath;

        if (TemplateProjectFolder != null &&
            _uiSettingsService.CodeGeneratorTemplateProject != TemplateProjectFolder.Name)
            _uiSettingsService.CodeGeneratorTemplateProject = TemplateProjectFolder.Name;

        DialogResult = ContentDialogResult.Primary;
    }

    private async Task PickOutputFolder()
    {
        var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.Desktop };

        // When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
        if (Window.Current == null)
        {
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, Win32Helper.GetActiveWindow());
        }

        var outputFolder = await folderPicker.PickSingleFolderAsync();
        if (outputFolder != null)
        {
            OutputPath = outputFolder.Path;
        }
    }

    protected override async void OnActivated()
    {
        OutputPath = _uiSettingsService.CodeGeneratorOutputPath;
        var folders = await _storageService.GetProjectFoldersAsync();
        if (folders is { Count: > 0 })
        {
            foreach (var storageFolder in folders)
            {
                if (storageFolder.Name == Constants.CodeTemplate.DefaultGlobalTemplateFolderName) continue;
                ProjectFolders.Add(storageFolder);
            }

            foreach (var fo in ProjectFolders)
            {
                if (fo.Name != _uiSettingsService.CodeGeneratorTemplateProject) continue;
                TemplateProjectFolder = fo;
                break;
            }

            if (TemplateProjectFolder == null) TemplateProjectFolder = ProjectFolders[0];
        }
    }
}