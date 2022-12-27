using Google.Protobuf.WellKnownTypes;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Channels;
using Enum = System.Enum;

namespace Netcool.DbToys.WinUI.Services;

public enum NotificationSeverity
{
    Informational,
    Success,
    Warning,
    Error
}
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
    public ValueTask QueueNotificationAsync(Notification notification);

    ValueTask<Notification> DequeueNotificationAsync(CancellationToken cancellationToken);
}

public class NotificationService : INotificationService
{
    private readonly Channel<Notification> _queue;

    public NotificationService()
    {
        _queue = Channel.CreateUnbounded<Notification>();
    }

    public async ValueTask QueueNotificationAsync(Notification notification)
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