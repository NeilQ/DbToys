
namespace DbToys.Core;

public static class Constants
{
    public static class LocalSettings
    {
        public const string DatabaseHistorySettingsFileName = "database.json";

        public const string GeneralSettingsFileName = "settings.json";

        public const string UiSettingsFileName = "ui.json";
    }

    public static class Notification
    {
        public const int ShortDuration = 3000;
        public const int DefaultDuration = 5000;
    }

    public static class FileSystem
    {
        public const string CachedEmptyItemName = "fileicon_cache";
        public const string DefaultApplicationDataFolderPath = "Netcool\\DbToys";
        public const string DefaultCodeTemplateFolderPath = "DbToys\\CodeTemplates";
        public const string CodeTemplateFileExtension = ".tpl";
    }

    public static class CodeTemplate
    {
        public const string InitialTemplateText = @"
{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | string.to_pascal_case | string.to_singular) + "".cs"" ~}} 
{{~ classname = table.clean_name | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
";
    }
}