using System.Threading.Channels;
using DbToys.Core;
using DbToys.Core.Log;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace DbToys.Services;

public class Notification
{
    public string Title { get; set; }
    public string Message { get; set; }
    public InfoBarSeverity Severity { get; set; }
    public int Duration { get; set; }
    public SolidColorBrush StrokeColor { get; private set; } = new(Colors.DeepSkyBlue);

    public Notification() { }

    public Notification(string title, string message)
    {
        Message = message;
        Severity = InfoBarSeverity.Informational;
        UpdateTitle(title);
        UpdateColor();
    }

    public Notification(string title, string message, InfoBarSeverity severity)
    {
        Message = message;
        Severity = severity;
        UpdateTitle(title);
        UpdateColor();
    }

    public Notification(string title, string message, InfoBarSeverity severity, int duration)
    {
        Message = message;
        Severity = severity;
        Duration = duration;
        UpdateTitle(title);
        UpdateColor();
    }

    private void UpdateTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            switch (Severity)
            {
                case InfoBarSeverity.Informational:
                    Title = "Info";
                    break;
                case InfoBarSeverity.Success:
                    Title = "Success";
                    break;
                case InfoBarSeverity.Warning:
                    Title = "Warning";
                    break;
                case InfoBarSeverity.Error:
                    Title = "Error";
                    break;
            }
        }
        else
        {
            Title = title;
        }
    }

    private void UpdateColor()
    {
        switch (Severity)
        {
            case InfoBarSeverity.Informational:
                StrokeColor = new SolidColorBrush(Colors.Blue);
                break;
            case InfoBarSeverity.Success:
                StrokeColor = new SolidColorBrush(Colors.Green);
                break;
            case InfoBarSeverity.Warning:
                StrokeColor = new SolidColorBrush(Colors.Yellow);
                break;
            case InfoBarSeverity.Error:
                StrokeColor = new SolidColorBrush(Colors.Red);
                break;
        }
    }
}
public interface INotificationService
{
    public void Success(string message);

    public void Success(string message, string title, int duration = Constants.Notification.ShortDuration);

    public void Info(string message);

    public void Info(string message, string title, int duration = Constants.Notification.DefaultDuration);

    public void Error(string message);

    public void Error(string message, string title, int duration = Constants.Notification.DefaultDuration);

    public void Warning(string message);

    public void Warning(string message, string title, int duration = Constants.Notification.DefaultDuration);

    public void QueueNotification(Notification notification);

    ValueTask<Notification> DequeueNotificationAsync(CancellationToken cancellationToken);
}

public class NotificationService : INotificationService
{
    private readonly Channel<Notification> _queue;

    public NotificationService()
    {
        _queue = Channel.CreateUnbounded<Notification>();
    }

    public void Success(string message)
    {
        Success(message, null);
    }

    public void Success(string message, string title, int duration = Constants.Notification.ShortDuration)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Success, duration));
    }

    public void Info(string message)
    {
        Info(message, null);
    }

    public void Info(string message, string title, int duration = Constants.Notification.DefaultDuration)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Informational, duration));
    }

    public void Error(string message)
    {
        Error(message, null);
    }

    public void Error(string message, string title, int duration = Constants.Notification.DefaultDuration)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Error, duration));
    }

    public void Warning(string message)
    {
        Warning(message, null);
    }

    public void Warning(string message, string title, int duration = Constants.Notification.DefaultDuration)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Warning, duration));
    }

    public async void QueueNotification(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        ArgumentException.ThrowIfNullOrEmpty(notification.Message);
        await _queue.Writer.WriteAsync(notification);
        switch (notification.Severity)
        {
            case InfoBarSeverity.Informational:
                Logger.Information(notification.Message);
                break;
            case InfoBarSeverity.Success:
                break;
            case InfoBarSeverity.Warning:
                Logger.Warning(notification.Message);
                break;
            case InfoBarSeverity.Error:
                Logger.Error(notification.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async ValueTask<Notification> DequeueNotificationAsync(CancellationToken cancellationToken)
    {
        var notification = await _queue.Reader.ReadAsync(cancellationToken);

        return notification;
    }
}