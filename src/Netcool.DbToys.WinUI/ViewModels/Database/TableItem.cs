using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Services;
using Netcool.DbToys.WinUI.Views.Dialogs;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class TableItem : TreeItem
{
    public Table Table { get; set; }

    private bool _loadingColumns;
    public bool LoadingColumns
    {
        get => _loadingColumns;
        set => SetProperty(ref _loadingColumns, value);
    }
    public IAsyncRelayCommand GenerateCodeCommand { get; set; }

    private readonly Lazy<CodeTemplateStorageService> _storageService = new(App.GetService<CodeTemplateStorageService>);

    public TableItem(Table table) : base(table.DisplayName, false)
    {
        Table = table;
        GenerateCodeCommand = new AsyncRelayCommand(GenerateCode);
    }

    private async Task GenerateCode()
    {
        var dialog = App.GetService<GenerateCodeDialog>();
        dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        var res = await dialog.ShowAsync();
        if (res != ContentDialogResult.Primary) return;
        if (dialog.ViewModel.SelectedFolder == null || dialog.ViewModel.OutputFolder == null) return;
        var files = await _storageService.Value.GetTemplateFilesAsync(dialog.ViewModel.SelectedFolder);
        if (files == null) return;

    }
}