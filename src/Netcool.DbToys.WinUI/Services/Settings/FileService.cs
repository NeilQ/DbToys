using System.Text;
using Netcool.DbToys.Helpers;

namespace Netcool.DbToys.Services;

public class FileService : IFileService
{
    public T Read<T>(string folderPath, string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(folderPath);
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return Json.Deserialize<T>(json);
        }

        return default;
    }

    public void Save<T>(string folderPath, string fileName, T content)
    {
        ArgumentException.ThrowIfNullOrEmpty(folderPath);
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = Json.Serialize(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(folderPath);
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        if (File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }
}
