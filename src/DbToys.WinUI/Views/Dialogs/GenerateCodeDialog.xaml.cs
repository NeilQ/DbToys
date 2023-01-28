using DbToys.Helpers;
using DbToys.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.Views.Dialogs
{
    public sealed partial class GenerateCodeDialog 
    {
        public GenerateCodeViewModel ViewModel { get; set; }

        public GenerateCodeDialog()
        {
            ViewModel = App.GetService<GenerateCodeViewModel>();
            ViewModel.IsActive = true;
            InitializeComponent();
        }

        public new Task<ContentDialogResult> ShowAsync() => DialogHelper.SetContentDialogRoot(this).ShowAsync().AsTask();

    }
}
