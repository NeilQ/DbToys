using System.Text.Json;

namespace Netcool.DbToys.WinUI.Helpers;

public static class Json
{
    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run<T>(() => JsonSerializer.Deserialize<T>(value));
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run<string>(() => JsonSerializer.Serialize(value));
    }
}
