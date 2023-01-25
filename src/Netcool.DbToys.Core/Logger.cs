using Serilog;

namespace Netcool.DbToys.Core;

public static class Logger
{

    private static readonly string ApplicationLogPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.FileSystem.DefaultApplicationDataFolderPath, "logs");

    static Logger()
    {
        var logFilePath = Path.Combine(ApplicationLogPath, "log.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }

    public static void Information(string message)
    {
        Log.Logger.Information(message);
    }

    public static void Error(string message)
    {
        Log.Logger.Error(message);
    }

    public static void Error(string message, Exception e)
    {
        Log.Logger.Error(e, message);
    }

    public static void Debug(string message)
    {
        Log.Logger.Debug(message);
    }

    public static void Debug(string message, Exception e)
    {
        Log.Logger.Debug(e, message);
    }

    public static void Warning(string message)
    {
        Log.Logger.Warning(message);
    }

}