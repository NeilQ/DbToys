using CommunityToolkit.Mvvm.ComponentModel;

namespace DbToys.ViewModels.Dialogs;

public class TemplateFilenameViewModel : ObservableObject
{
    private string _title;
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    private string _filename;
    public string Filename { get => _filename; set => SetProperty(ref _filename, value); }

    private bool _canConfirm;
    public bool CanConfirm { get => _canConfirm; set => SetProperty(ref _canConfirm, value); }

    public TemplateFilenameViewModel()
    {

    }

}