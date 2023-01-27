using Netcool.DbToys.Core;

namespace Netcool.DbToys.Helpers;

public static class FileSystemHelper
{
    private static readonly char[] RestrictedCharacters = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
    private static readonly string[] RestrictedFileNames = {
        "CON", "PRN", "AUX",
        "NUL", "COM1", "COM2",
        "COM3", "COM4", "COM5",
        "COM6", "COM7", "COM8",
        "COM9", "LPT1", "LPT2",
        "LPT3", "LPT4", "LPT5",
        "LPT6", "LPT7", "LPT8", "LPT9"
    };
    
    public static string GetDbToysLogFolder()
    {
        var folder =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.FileSystem.DefaultApplicationDataFolderPath, "Logs");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        return folder;
    }

    public static string GetDbToysAppDataFolder()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, Constants.FileSystem.DefaultApplicationDataFolderPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        return folder;
    }

    public static string FilterRestrictedCharacters(string input)
    {
        int invalidCharIndex;
        while ((invalidCharIndex = input.IndexOfAny(RestrictedCharacters)) >= 0)
        {
            input = input.Remove(invalidCharIndex, 1);
        }
        return input;
    }

    public static bool ContainsRestrictedCharacters(string input)
    {
        return input.IndexOfAny(RestrictedCharacters) >= 0;
    }

    public static bool ContainsRestrictedFileName(string input)
    {
        foreach (string name in RestrictedFileNames)
        {
            if (input.StartsWith(name, StringComparison.OrdinalIgnoreCase) && (input.Length == name.Length || input[name.Length] == '.'))
                return true;
        }

        return false;
    }

}