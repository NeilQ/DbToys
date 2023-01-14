
namespace Netcool.DbToys.WinUI.Helpers;

public static class FileSystemHelper
{
    public static string GetDbToysAppDataFolder()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, Constants.FileSystem.DefaultApplicationDataFolderPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        return folder;
    }
}