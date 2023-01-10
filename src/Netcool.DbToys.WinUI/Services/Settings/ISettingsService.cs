namespace Netcool.DbToys.WinUI.Services;

public interface ISettingsService
{
    string SettingFileName { get; set; }

    T ReadSetting<T>(string key);

    void SaveSetting<T>(string key);

    Task<T> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);
}
