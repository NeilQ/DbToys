namespace Netcool.DbToys.WinUI;

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
        public const string DefaultApplicationDataFolderPath = "Netcool/DbToys";
        public const string DefaultCodeTemplateFolderPath = "DbToys\\CodeTemplates";
        public const string CodeTemplateFileExtension = ".tpl";
    }
}