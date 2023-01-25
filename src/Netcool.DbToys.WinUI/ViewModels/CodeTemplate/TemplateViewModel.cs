using Windows.Storage;
using Windows.Storage.Streams;
using CommunityToolkit.Mvvm.ComponentModel;
using Netcool.DbToys.Services;

namespace Netcool.DbToys.ViewModels.CodeTemplate;

public class TemplateViewModel : ObservableObject
{
    public StorageFile File { get; set; }

    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>);

    public async Task<string> ReadTextAsync()
    {
        if (File == null) return string.Empty;
        return await FileIO.ReadTextAsync(File, UnicodeEncoding.Utf8);
    }

    public async void SaveText(string text)
    {
        try
        {
            await FileIO.WriteTextAsync(File, text);
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Save file failed: {ex.Message}");
            return;
        }
        _notificationService.Value.Success("File saved");
    }
}