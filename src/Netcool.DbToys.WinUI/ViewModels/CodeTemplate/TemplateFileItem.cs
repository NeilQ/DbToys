
using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Netcool.DbToys.WinUI.Helpers;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public class TemplateFileItem : TreeItem
{
    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFile File { get; set; }

    public TemplateFileItem(StorageFile file, bool lazyLoadChildren) : base(file.DisplayName, lazyLoadChildren)
    {
        File = file;
    }

    public async void LoadIcon()
    {
        var thumbnail = await File.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
        var img = new BitmapImage();
        img.SetSource(thumbnail);
        Icon = img;
    }
}