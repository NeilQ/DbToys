
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Netcool.DbToys.WinUI.Helpers;

public static class Json
{
    private static JsonSerializerOptions _options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    public static T Deserialize<T>(string value)
    {
        var type = typeof(T);
        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsPrimitive || type == typeof(string))
        {
            return (T)Convert.ChangeType(value, type);
        }

        return JsonSerializer.Deserialize<T>(value, _options);
    }

    public static string Serialize(object value)
    {
        var type = value.GetType();
        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsPrimitive || type == typeof(string))
        {
            return (string)value;
        }
        return JsonSerializer.Serialize(value, _options);
    }

    public static async Task<T> DeserializeAsync<T>(string value)
    {
        return await Task.Run(() => Deserialize<T>(value));
    }

    public static async Task<string> SerializeAsync(object value)
    {
        return await Task.Run(() => Serialize(value));
    }
}
