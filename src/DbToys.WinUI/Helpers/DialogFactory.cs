using CommunityToolkit.WinUI;
using DbToys.ViewModels.Dialogs;
using DbToys.Views.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.Helpers;

public class DialogFactory
{
    public static DynamicDialog GetFor_DeleteProjectConfirmDialog()
    {
        return GetFor_ConfirmDialog("Delete Project Folder", "Are you sure you want move this folder to Recycle Bin?");
    }
    public static DynamicDialog GetFor_DeleteTemplateConfirmDialog()
    {
        return GetFor_ConfirmDialog("Delete Template File", "Are you sure you want move this file to Recycle Bin?");
    }

    public static DynamicDialog GetFor_ConfirmDialog(string title, string prompt)
    {
        DynamicDialog dialog = new DynamicDialog(new DynamicDialogViewModel
        {
            Title = title,
            SubTitle = prompt,
            PrimaryButtonText = "Confirm",
            CloseButtonText = "Cancel"
        });
        return dialog;
    }

    public static DynamicDialog GetFor_FilenameDialog(string originFilename, string dialogTitle, string dialogSubTitle, string tooltip, string inputPlaceHolder)
    {
        DynamicDialog dialog = null;
        var inputText = new TextBox
        {
            Height = 35d,
            MaxLength = 255,
            PlaceholderText = inputPlaceHolder,
            Text = originFilename
        };
        var tipText = new TextBlock
        {
            Text = tooltip,
            Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 4, 0),
            TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
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
            AdditionalData = originFilename,
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
                                inputText,
                                tipText
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
        return GetFor_FilenameDialog(originName, "Rename", null, null, null);
    }

    public static DynamicDialog GetFor_CreateTemplateDialog()
    {
        return GetFor_FilenameDialog(null, "Create Template ", null, null, null);
    }

    public static DynamicDialog GetFor_CreateProjectDialog()
    {
        return GetFor_FilenameDialog(null, "Create Project", null, null, null);
    }

}