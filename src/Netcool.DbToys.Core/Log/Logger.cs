using System.Globalization;
using System.Threading.Channels;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Netcool.DbToys.Core.Log;

public static class Logger
{
    public static readonly string ApplicationLogPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.FileSystem.DefaultApplicationDataFolderPath, "Logs");
    //  private static readonly LogEventSink Sink = new();

    static Logger()
    {
        var logFilePath = Path.Combine(ApplicationLogPath, ".md");
        Serilog.Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Information()
             .WriteTo.Debug()
             //.WriteTo.Sink(Sink)
             .WriteTo.File(new MarkdownFormatter(), logFilePath,
                 rollingInterval: RollingInterval.Day,
                 rollOnFileSizeLimit: true)
             .CreateLogger();
    }

    public static void Information(string message)
    {
        Serilog.Log.Logger.Information(message);
    }

    public static void Error(string message)
    {
        Serilog.Log.Logger.Error(message);
    }

    public static void Error(string message, Exception e)
    {
        Serilog.Log.Logger.Error(e, message);
    }

    public static void Debug(string message)
    {
        Serilog.Log.Logger.Debug(message);
    }

    public static void Debug(string message, Exception e)
    {
        Serilog.Log.Logger.Debug(e, message);
    }

    public static void Warning(string message)
    {
        Serilog.Log.Logger.Warning(message);
    }
}

class MarkdownFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        output.Write($"**{logEvent.Timestamp.ToLocalTime().ToString($"yyyy-MM-dd HH:mm:ss.fff zzz")}** ");
        if (logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal)
        {
            output.Write($"***[{logEvent.Level}]*** ");
        }
        else
        {
            output.Write($"**[{logEvent.Level}]** ");
        }
        output.WriteLine($"{logEvent.RenderMessage()}  ");
        if (logEvent.Exception != null)
        {
            output.WriteLine($"{logEvent.Exception}  ");
        }
        output.Flush();
    }
}

class LogEventSink : ILogEventSink
{
    readonly ITextFormatter _textFormatter;

    public readonly Channel<string> Queue = Channel.CreateUnbounded<string>();

    public LogEventSink()
    {
        _textFormatter = new MessageTemplateTextFormatter(DefaultOutputTemplate);
    }

    const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage(new DateTimeFormatInfo());
        var sr = new StringWriter();
        _textFormatter.Format(logEvent, sr);
        Queue.Writer.TryWrite(sr + Environment.NewLine);
    }
}