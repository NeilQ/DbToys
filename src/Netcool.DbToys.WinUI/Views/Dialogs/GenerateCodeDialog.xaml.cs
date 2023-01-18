using Netcool.DbToys.WinUI.ViewModels.Dialogs;

namespace Netcool.DbToys.WinUI.Views.Dialogs
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
    }
}
