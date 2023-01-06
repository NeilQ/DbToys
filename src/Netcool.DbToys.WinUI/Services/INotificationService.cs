using Microsoft.UI.Xaml.Controls;
using System.Threading.Channels;

namespace Netcool.DbToys.WinUI.Services;

public class Notification
{
    public string Title { get; set; }
    public string Message { get; set; }
    public InfoBarSeverity Severity { get; set; }
    public int Duration { get; set; }

    public Notification() { }

    public Notification(string title, string message)
    {
        Title = title;
        Message = message;
    }

    public Notification(string title, string message, InfoBarSeverity severity)
    {
        Title = title;
        Message = message;
        Severity = severity;
    }

    public Notification(string title, string message, InfoBarSeverity severity, int duration)
    {
        Title = title;
        Message = message;
        Severity = severity;
        Duration = duration;
    }
}
public interface INotificationService
{
    public void Success(string message, string title = null, int duration = 5000);

    public void Info(string message, string title = null, int duration = 0);

    public void Error(string message, string title = null, int duration = 0);

    public void Warning(string message, string title = null, int duration = 0);

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

    public void Success(string message, string title, int duration = 5000)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Success, duration));
    }

    public void Info(string message, string title, int duration = 0)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Informational, duration));
    }

    public void Error(string message, string title, int duration = 0)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Error, duration));
    }

    public void Warning(string message, string title, int duration = 0)
    {
        QueueNotification(new(title, message, InfoBarSeverity.Warning, duration));
    }

    public async void QueueNotification(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        ArgumentException.ThrowIfNullOrEmpty(notification.Message);
        await _queue.Writer.WriteAsync(notification);
    }

    public async ValueTask<Notification> DequeueNotificationAsync(CancellationToken cancellationToken)
    {
        var notification = await _queue.Reader.ReadAsync(cancellationToken);

        return notification;
    }
}