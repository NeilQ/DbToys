using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.ViewModels.Dialogs;
using Netcool.DbToys.WinUI.Views.Dialogs;

namespace Netcool.DbToys.WinUI.Helpers;

public class DynamicDialogFactory
{

    public static DynamicDialog GetFor_FilenameDialog(string originFilename, string dialogTitle, string dialogSubTitle, string inputPlaceHolder)
    {
        DynamicDialog dialog = null;
        TextBox inputText = new TextBox
        {
            Height = 35d,
            PlaceholderText = inputPlaceHolder,
            Text = originFilename
        };

        inputText.BeforeTextChanging += async (textBox, args) =>
        {
            if (FileSystemHelper.ContainsRestrictedCharacters(args.NewText))
            {
                args.Cancel = true;
                await inputText.DispatcherQueue.EnqueueAsync(() =>
                {
                    var oldSelection = textBox.SelectionStart + textBox.SelectionLength;
                    var oldText = textBox.Text;
                    textBox.Text = FileSystemHelper.FilterRestrictedCharacters(args.NewText);
                    textBox.SelectionStart = oldSelection + textBox.Text.Length - oldText.Length;
                });
            }
            else
            {
                dialog!.ViewModel.AdditionalData = args.NewText;
                dialog!.ViewModel.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(args.NewText);
            }
        };

        inputText.Loaded += (s, e) =>
        {
            // dispatching to the ui thread fixes an issue where the primary dialog button would steal focus
            _ = inputText.DispatcherQueue.EnqueueAsync(() => inputText.Focus(Microsoft.UI.Xaml.FocusState.Programmatic));
        };

        dialog = new DynamicDialog(new DynamicDialogViewModel
        {
            Title = dialogTitle,
            SubTitle = dialogSubTitle,
            DisplayControl = new Grid
            {
                MinWidth = 300d,
                Children =
                    {
                        new StackPanel
                        {
                            Spacing = 4d,
                            Children =
                            {
                                inputText
                                //tipText
                            }
                        }
                    }
            },
            PrimaryButtonAction = (vm, _) =>
            {
                vm.HideDialog();
            },
            PrimaryButtonText = "Confirm",
            CloseButtonText = "Cancel",
        });

        return dialog;
    }

    public static DynamicDialog GetFor_RenameDialog(string originName)
    {
        return GetFor_FilenameDialog(originName, "Rename", null, null);
    }

    public static DynamicDialog GetFor_CreateTemplateDialog()
    {
        return GetFor_FilenameDialog(null, "Create Template", null, null);
    }

    public static DynamicDialog GetFor_CreateProjectDialog()
    {
        return GetFor_FilenameDialog(null, "Create Project", null, null);
    }

}