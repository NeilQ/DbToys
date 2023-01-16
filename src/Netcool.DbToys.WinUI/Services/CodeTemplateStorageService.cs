using Windows.Storage;
using Windows.Storage.Search;

namespace Netcool.DbToys.WinUI.Services;

public class CodeTemplateStorageService
{
    private readonly string _templateFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Constants.FileSystem.DefaultCodeTemplateFolderPath);

    public string TemplateFolderPath => _templateFolderPath;

    public async Task<List<StorageFolder>> GetProjectFoldersAsync()
    {
        EnsureTemplateFolder();
        var templateFolder = await StorageFolder.GetFolderFromPathAsync(_templateFolderPath);
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
        if (!Directory.Exists(_templateFolderPath))
        {
            try
            {
                Directory.CreateDirectory(TemplateFolderPath);
            }
            catch (Exception) { }
        }
    }
}