using Microsoft.Extensions.Options;
using Netcool.DbToys.WinUI.Models;

namespace Netcool.DbToys.WinUI.Services;

public class GeneralSettingsService : SettingsServiceBase
{
    public GeneralSettingsService(IFileService fileService, IOptions<SettingsOptions> options)
        : base(fileService,options)
    {
    }

    public override string SettingFileName { get; set; } = "settings.json";
}