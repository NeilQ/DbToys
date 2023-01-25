using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.Helpers;
using Netcool.DbToys.ViewModels.Dialogs;

namespace Netcool.DbToys.Views.Dialogs
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
