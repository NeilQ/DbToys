using Windows.Storage;
using Windows.Storage.Search;

namespace Netcool.DbToys.WinUI.Services;

public class CodeTemplateStorageService
{
    private static readonly string TemplateFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Constants.FileSystem.DefaultCodeTemplateFolderPath);

    public async Task<List<StorageFolder>> GetProjectFoldersAsync()
    {
        EnsureTemplateFolder();
        var templateFolder = await StorageFolder.GetFolderFromPathAsync(TemplateFolderPath);
        var folders = await templateFolder.GetFoldersAsync();
        return folders.ToList();
    }

    public async Task<List<StorageFile>> GetTemplateFiles(StorageFolder folder)
    {
        ArgumentNullException.ThrowIfNull(nameof(folder));
        var files = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
        return files.ToList();
    }

    private void EnsureTemplateFolder()
    {
        if (!Directory.Exists(TemplateFolderPath))
        {
            try
            {
                Directory.CreateDirectory(TemplateFolderPath);
            }
            catch (Exception) { }
        }
    }
}