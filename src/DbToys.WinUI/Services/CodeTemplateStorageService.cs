using System.Text;
using Windows.Storage;
using Windows.Storage.Search;
using DbToys.Core;
using System.Text.RegularExpressions;

namespace DbToys.Services;

public class CodeTemplateStorageService
{
    public string TemplateFolderPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Constants.FileSystem.DefaultCodeTemplateFolderPath);

    public StorageFolder TemplateFolder { get; private set; }

    public async Task<IReadOnlyList<StorageFolder>> GetProjectFoldersAsync()
    {
        await EnsureTemplateFolder();
        await EnsureGlobalProject();
        var folders = await TemplateFolder.GetFoldersAsync();
        return folders.ToList();
    }

    public async Task<List<StorageFile>> GetTemplateFilesAsync(StorageFolder folder)
    {
        ArgumentNullException.ThrowIfNull(nameof(folder));
        var files = (await folder.GetFilesAsync(CommonFileQuery.OrderByName)).ToList();
        if (folder.Name == Constants.CodeTemplate.DefaultGlobalTemplateFolderName && files.Count == 0)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/global.tpl"));
            var defaultGlobalTemplate = await file.CopyAsync(folder, file.Name);

            files.Add(defaultGlobalTemplate);
        }
        return files;
    }

    public async Task<StorageFolder> CreateProjectFolder(string folderName)
    {
        if (string.IsNullOrEmpty(folderName)) return null;
        await EnsureTemplateFolder();
        if (Directory.Exists(Path.Join(TemplateFolderPath, folderName))) return null;
        var folder = await TemplateFolder.CreateFolderAsync(folderName);
        return folder;
    }

    public async Task<StorageFile> CreateTemplateFile(StorageFolder folder, string filename, string initText)
    {
        if (folder == null || string.IsNullOrEmpty(filename)) return null;

        var file = await folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
        if (!string.IsNullOrEmpty(initText))
        {
            await FileIO.WriteTextAsync(file, Constants.CodeTemplate.InitialTemplateText);
        }

        return file;
    }

    public async Task<string> GetGlobalTemplateText()
    {
        var folder = await TemplateFolder.GetFolderAsync(Constants.CodeTemplate.DefaultGlobalTemplateFolderName);
        var files = await folder.GetFilesAsync();
        if (files == null || files.Count == 0) return null;
        var sb = new StringBuilder();
        foreach (var file in files)
        {
            var text = await FileIO.ReadTextAsync(file);
            if (string.IsNullOrWhiteSpace(text)) continue;
            sb.AppendLine(Regex.Replace(text, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).Trim());
        }

        return sb.ToString();
    }


    private async Task EnsureTemplateFolder()
    {
        if (TemplateFolder != null) return;

        if (!Directory.Exists(TemplateFolderPath))
        {
            Directory.CreateDirectory(TemplateFolderPath);
        }

        TemplateFolder = await StorageFolder.GetFolderFromPathAsync(TemplateFolderPath);
    }

    private async Task EnsureGlobalProject()
    {
        var item = await TemplateFolder.TryGetItemAsync(Constants.CodeTemplate.DefaultGlobalTemplateFolderName);
        if (item is StorageFolder)
        {
            return;
        }

        await TemplateFolder.CreateFolderAsync(Constants.CodeTemplate.DefaultGlobalTemplateFolderName);
    }
}