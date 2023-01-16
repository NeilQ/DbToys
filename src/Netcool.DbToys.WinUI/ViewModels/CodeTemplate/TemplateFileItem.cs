using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public class TemplateFileItem : TreeItem
{
    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFile File { get; set; }

    public StorageFolder Folder { get; set; }

    public string TabDisplayName => $"{Folder.Name}\\{File.Name}";

    public TemplateFileItem(StorageFile file, StorageFolder folder) : base(file.Name, false)
    {
        File = file;
        Folder = folder;
        LoadIcon();
    }

    public async void LoadIcon()
    {
        var thumbnail = await File.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
        var img = new BitmapImage();
        img.SetSource(thumbnail);
        Icon = img;
    }
}