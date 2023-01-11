namespace Netcool.DbToys.WinUI.Services;

public interface ISettingsService
{
    string SettingFileName { get; set; }

    T GetValue<T>(string key);

    void SetValue<T>(string key, T value);

    Task<T> GetValueAsync<T>(string key);

    Task SetValueAsync<T>(string key, T value);
}
