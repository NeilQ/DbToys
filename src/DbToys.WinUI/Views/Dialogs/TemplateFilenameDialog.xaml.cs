using CommunityToolkit.WinUI;
using DbToys.Helpers;
using DbToys.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.Views.Dialogs
{
    public sealed partial class TemplateFilenameDialog
    {
        public TemplateFilenameViewModel ViewModel { get; set; }

        public TemplateFilenameDialog()
        {
            ViewModel = App.GetService<TemplateFilenameViewModel>();
            InitializeComponent();
        }

        public new Task<ContentDialogResult> ShowAsync() => DialogHelper.SetContentDialogRoot(this).ShowAsync().AsTask();

        private async void FilenameTextBox_OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (!FileSystemHelper.ContainsRestrictedCharacters(args.NewText)) return;

            args.Cancel = true;
            await sender.DispatcherQueue.EnqueueAsync(() =>
            {
                var oldSelection = sender.SelectionStart + sender.SelectionLength;
                var oldText = sender.Text;
                sender.Text = FileSystemHelper.FilterRestrictedCharacters(args.NewText);
                sender.SelectionStart = oldSelection + sender.Text.Length - oldText.Length;
            });
        }
    }
}
