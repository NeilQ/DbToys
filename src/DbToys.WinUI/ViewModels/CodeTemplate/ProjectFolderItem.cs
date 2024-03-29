﻿using Windows.Storage;
using Windows.Storage.FileProperties;
using CommunityToolkit.Mvvm.Input;
using DbToys.Core;
using DbToys.Helpers;
using DbToys.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DbToys.ViewModels.CodeTemplate;

public class ProjectFolderItem : TreeItem
{
    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>);
    private readonly Lazy<CodeTemplateStorageService> _templateStorageService = new(App.GetService<CodeTemplateStorageService>);

    private bool _canDelete = true;
    public bool CanDelete { get => _canDelete; set => SetProperty(ref _canDelete, value); }

    private bool _canRename = true;
    public bool CanRename { get => _canRename; set => SetProperty(ref _canRename, value); }

    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFolder Folder { get; set; }

    public Action<RenamedArgs> RenamedAction { get; set; }

    public Action<ProjectDeletedArg> DeletedAction { get; set; }

    public Action<TemplateCreatedArg> TemplateCreatedAction { get; set; }

    public IAsyncRelayCommand CreateTemplateCommand { get; set; }

    public IAsyncRelayCommand RenameCommand { get; set; }

    public IAsyncRelayCommand DeleteCommand { get; set; }

    public ProjectFolderItem(StorageFolder folder, bool lazyLoadChildren) : base(folder.Name, lazyLoadChildren)
    {
        Folder = folder;
        if (folder.Name == Constants.CodeTemplate.DefaultGlobalTemplateFolderName)
        {
            Name = "Global";
            CanDelete = false;
            CanRename = false;
        }
        CreateTemplateCommand = new AsyncRelayCommand(CreateTemplateFile);
        RenameCommand = new AsyncRelayCommand(RenameAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        LoadIcon();
    }

    private async Task DeleteAsync()
    {
        var dialog = DialogFactory.GetFor_DeleteProjectConfirmDialog();
        await dialog.ShowAsync();
        if (dialog.ViewModel.DialogResult != ContentDialogResult.Primary)
            return;

        await Folder.DeleteAsync();

        DeletedAction?.Invoke(new ProjectDeletedArg(Folder.Name, Folder.Path));
    }

    private async Task RenameAsync()
    {
        var oldName = Folder.Name;
        var oldPath = Folder.Path;
        var dialog = DialogFactory.GetFor_RenameDialog(Folder.Name);
        await dialog.ShowAsync();
        string newName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            newName = (string)dialog.ViewModel.AdditionalData;
        }
        else return;

        if (string.IsNullOrEmpty(newName) || oldName == newName) return;
        try
        {
            await Folder.RenameAsync(newName);
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Rename project folder failed with error: {ex.Message}");
            return;
        }
        RenamedAction?.Invoke(new RenamedArgs(oldName, newName, oldPath, Folder.Path));
    }

    private async Task CreateTemplateFile()
    {
        var dialog = DialogFactory.GetFor_CreateTemplateDialog();
        await dialog.ShowAsync();
        string fileName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            var input = (string)dialog.ViewModel.AdditionalData;
            if (input.EndsWith(Constants.FileSystem.CodeTemplateFileExtension))
            {
                fileName = input;
            }
            else
            {
                fileName = input + Constants.FileSystem.CodeTemplateFileExtension;
            }
        }
        else return;

        if (string.IsNullOrEmpty(fileName)) return;

        StorageFile file;
        try
        {
            file = await _templateStorageService.Value.CreateTemplateFile(Folder, fileName,
                Constants.CodeTemplate.InitialTemplateText);
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Create file failed with error: {ex.Message}");
            return;
        }
        TemplateCreatedAction?.Invoke(new TemplateCreatedArg(file));
    }

    public async void LoadIcon()
    {
        var img = new BitmapImage();
        if (Folder.Name == Constants.CodeTemplate.DefaultGlobalTemplateFolderName)
        {
            img.UriSource = new Uri("ms-appx:///Assets/Icons/chain_start.png");
        }
        else
        {
            var thumbnail = await Folder.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
            img.SetSource(thumbnail);
        }
        Icon = img;
    }
}