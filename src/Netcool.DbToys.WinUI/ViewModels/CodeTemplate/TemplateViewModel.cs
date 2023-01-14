using Windows.Storage;
using Windows.Storage.Streams;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public class TemplateViewModel : ObservableObject
{
    public StorageFile File { get; set; }

    private string _text;
    public string Text { get => _text; set => SetProperty(ref _text, value); }

    public async void LoadFile(StorageFile file)
    {
        File = file;
        Text = await FileIO.ReadTextAsync(File, UnicodeEncoding.Utf8);
    }
}