using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Storage;
using Netcool.DbToys.WinUI.Helpers;

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

    protected SettingsServiceBase(IFileService fileService)
    {
        _fileService = fileService;

        _applicationDataFolder = Path.Combine(_localApplicationData,DefaultApplicationDataFolder);
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

    public T GetValue<T>(string key)
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

            if (_settings == null || !_settings.TryGetValue(key, out var obj)) return default;

            if (obj is JsonElement jElem)
            {
                return jElem.Deserialize<T>();
            }

            return (T)obj;
        }

        return default;
    }

    public void SetValue<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
        else
        {
            Initialize();

            _settings[key] = value;

            _fileService.Save(_applicationDataFolder, SettingFileName, _settings);
        }
    }

    public async Task<T> GetValueAsync<T>(string key)
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

            if (_settings == null || !_settings.TryGetValue(key, out var obj)) return default;

            if (obj is JsonElement jElem)
            {
                return jElem.Deserialize<T>();
            }

            return (T)obj;
        }

        return default;
    }

    public async Task SetValueAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
        else
        {
            await InitializeAsync();

            _settings[key] = value;

            await Task.Run(() => _fileService.Save(_applicationDataFolder, SettingFileName, _settings));
        }
    }
}