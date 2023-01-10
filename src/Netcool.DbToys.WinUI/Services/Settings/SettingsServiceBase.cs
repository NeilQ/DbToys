using System.Text.Json.Serialization;
using Windows.Storage;
using Microsoft.Extensions.Options;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.Models;

namespace Netcool.DbToys.WinUI.Services;

public abstract class SettingsServiceBase : ISettingsService
{
    private const string DefaultApplicationDataFolder = "Netcool/DbToys";

    [JsonIgnore]
    public abstract string SettingFileName { get; set; }

    private readonly IFileService _fileService;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    protected SettingsServiceBase(IFileService fileService, IOptions<SettingsOptions> options)
    {
        _fileService = fileService;

        _applicationDataFolder = Path.Combine(_localApplicationData, options.Value.ApplicationDataFolder ?? DefaultApplicationDataFolder);
        _settings = new Dictionary<string, object>();
    }

    protected virtual void Initialize()
    {
        if (!_isInitialized)
        {
            _settings = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, SettingFileName) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    protected virtual async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, SettingFileName)) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public T ReadSetting<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return Json.Deserialize<T>(obj.ToString());
            }
        }
        else
        {
            Initialize();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return Json.Deserialize<T>(obj.ToString());
            }
        }

        return default;
    }

    public void SaveSetting<T>(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<T> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.DeserializeAsync<T>(obj.ToString());
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.DeserializeAsync<T>(obj.ToString());
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.SerializeAsync(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.SerializeAsync(value);

            await Task.Run(() => _fileService.Save(_applicationDataFolder, SettingFileName, _settings));
        }
    }
}