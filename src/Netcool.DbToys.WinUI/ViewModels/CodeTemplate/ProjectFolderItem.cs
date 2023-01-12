using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public class ProjectFolderItem : TreeItem
{
    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFolder Folder { get; set; }

    public ProjectFolderItem(StorageFolder folder, bool lazyLoadChildren) : base(folder.DisplayName, lazyLoadChildren)
    {
        Folder = folder;
    }
    public async void LoadIcon()
    {
        var thumbnail = await Folder.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
        var img = new BitmapImage();
        img.SetSource(thumbnail);
        Icon = img;
    }
}