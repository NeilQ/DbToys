using Windows.Storage;
using Windows.Storage.FileProperties;
using Netcool.DbToys.Core;

namespace Netcool.DbToys.Helpers;

public class StorageItemIconHelper
{
    public static async Task<StorageItemThumbnail> GetIconForItemType(uint requestedSize, string fileExtension = null)
    {
        if (string.IsNullOrEmpty(fileExtension))
        {
            var localFolder = ApplicationData.Current.RoamingFolder;
            return await localFolder.GetThumbnailAsync(ThumbnailMode.ListView, requestedSize, ThumbnailOptions.UseCurrentScale);
        }

        var emptyFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(string.Concat(Constants.FileSystem.CachedEmptyItemName, fileExtension), CreationCollisionOption.OpenIfExists);
        var icon = await emptyFile.GetThumbnailAsync(ThumbnailMode.ListView, requestedSize, ThumbnailOptions.UseCurrentScale);

        return icon;
    }
}