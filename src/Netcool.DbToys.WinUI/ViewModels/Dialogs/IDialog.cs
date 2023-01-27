using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace Netcool.DbToys.ViewModels.Dialogs;

public interface IDialog<TViewModel> where TViewModel : class, INotifyPropertyChanged
{
    TViewModel ViewModel { get; set; }

    Task<ContentDialogResult> ShowAsync();
}