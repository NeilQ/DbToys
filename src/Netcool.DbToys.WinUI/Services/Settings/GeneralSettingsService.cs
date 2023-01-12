
namespace Netcool.DbToys.WinUI.Services;

public class GeneralSettingsService : SettingsServiceBase
{
    public GeneralSettingsService(IFileService fileService)
        : base(fileService)
    {
    }

    public override string SettingFileName { get; set; } = Constants.LocalSettings.GeneralSettingsFileName;
}