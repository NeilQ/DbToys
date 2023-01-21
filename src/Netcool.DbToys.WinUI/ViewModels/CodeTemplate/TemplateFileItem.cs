using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.Services;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public class TemplateFileItem : TreeItem
{
    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>);

    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFile File { get; set; }

    public StorageFolder Folder { get; set; }

    public string TabDisplayName => $"{Folder.Name}\\{File.Name}";

    public Action<RenamedArgs> RenamedAction { get; set; }

    public Action<TemplateDeletedArg> DeletedAction { get; set; }

    public IAsyncRelayCommand RenameCommand { get; set; }

    public IAsyncRelayCommand DeleteCommand { get; set; }

    public TemplateFileItem(StorageFile file, StorageFolder folder) : base(file.Name)
    {
        File = file;
        Folder = folder;
        RenameCommand = new AsyncRelayCommand(RenameAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        LoadIcon();
    }

    private async Task DeleteAsync()
    {
        var dialog = DialogFactory.GetFor_DeleteTemplateConfirmDialog();
        await dialog.ShowAsync();
        if (dialog.ViewModel.DialogResult != ContentDialogResult.Primary)
            return;

        await File.DeleteAsync();
        DeletedAction?.Invoke(new TemplateDeletedArg(File.Name, File.Path, Folder.Name, Folder.Path));
    }

    private async Task RenameAsync()
    {
        var oldName = File.Name;
        var oldPath = File.Path;
        string input;

        var templateFileExtension = Constants.FileSystem.CodeTemplateFileExtension;

        if (oldName.EndsWith(templateFileExtension))
        {
            input = oldName
                .Remove(oldName.Length - templateFileExtension.Length, templateFileExtension.Length);
        }
        else
        {
            input = oldName;
        }
        var dialog = DialogFactory.GetFor_RenameDialog(input);
        await dialog.ShowAsync();

        string newName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            input = (string)dialog.ViewModel.AdditionalData;
            if (input.EndsWith(Constants.FileSystem.CodeTemplateFileExtension))
            {
                newName = input;
            }
            else
            {
                newName = input + Constants.FileSystem.CodeTemplateFileExtension;
            }
        }
        else return;

        if (string.IsNullOrEmpty(newName) || oldName == newName) return;
        try
        {
            await File.RenameAsync(newName, NameCollisionOption.FailIfExists);
            // newName = File.Name; // Unique name may be generated
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Rename template file failed with error: {ex.Message}",
                Constants.Notification.DefaultDuration);
            return;
        }
        RenamedAction?.Invoke(new RenamedArgs(oldName, newName, oldPath, File.Path));
    }

    public async void LoadIcon()
    {
        var thumbnail = await File.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
        var img = new BitmapImage();
        img.SetSource(thumbnail);
        Icon = img;
    }
}