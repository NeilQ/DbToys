namespace Netcool.DbToys.WinUI.Services;

public interface ISettingsService
{
    string SettingFileName { get; set; }

    Task<T> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);
}
