
using DbToys.Core;

namespace DbToys.Services.Settings;

public class GeneralSettingsService : SettingsServiceBase
{
    public GeneralSettingsService(IFileService fileService)
        : base(fileService)
    {
    }

    public override string SettingFileName { get; set; } = Constants.LocalSettings.GeneralSettingsFileName;
}