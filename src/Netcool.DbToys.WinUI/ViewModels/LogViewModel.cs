using Windows.Storage;
using Windows.Storage.Search;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Netcool.DbToys.Core.Log;

namespace Netcool.DbToys.ViewModels;

public class LogViewModel : ObservableRecipient, INavigationAware
{
    private string _message;
    public string Message { get => _message; set => SetProperty(ref _message, value); }

    private StorageFolder _folder;

    //public IAsyncRelayCommand ReadLogCommand { get; set; }

    public LogViewModel()
    {
        // ReadLogCommand = new AsyncRelayCommand(ReadLogAsync);
    }

    protected override void OnActivated()
    {

    }

    public async void ReadLogAsync()
    {
        try
        {
            if (_folder == null)
            {
                _folder = await StorageFolder.GetFolderFromPathAsync(Logger.ApplicationLogPath);
            }

            if (_folder == null) return;
            var files = await _folder.GetFilesAsync(CommonFileQuery.OrderByName);
            if (files == null || files.Count == 0) return;
            var logFile = files.Last();
            Message = await FileIO.ReadTextAsync(logFile);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message, ex);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        ReadLogAsync();
    }

    public void OnNavigatedFrom()
    {
    }
}