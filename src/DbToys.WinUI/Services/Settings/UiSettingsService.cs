using DbToys.Core;

namespace DbToys.Services.Settings;

public class UiSettingsService : SettingsServiceBase
{
    public override string SettingFileName { get; set; } = Constants.LocalSettings.UiSettingsFileName;

    public string CodeGeneratorTemplateProject
    {
        get => GetValue<string>(nameof(CodeGeneratorTemplateProject));
        set => SetValue(nameof(CodeGeneratorTemplateProject), value);
    }

    public string CodeGeneratorOutputPath
    {
        get => GetValue<string>(nameof(CodeGeneratorOutputPath));
        set => SetValue(nameof(CodeGeneratorOutputPath), value);
    }

    public UiSettingsService(IFileService fileService) : base(fileService) { }

}