using Windows.Storage;
using Windows.Storage.Streams;
using CommunityToolkit.Mvvm.Input;
using DbToys.Core;
using DbToys.Core.Database;
using DbToys.Services;
using DbToys.Views.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace DbToys.ViewModels.Database;

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
    private readonly Lazy<ICodeGenerator> _codeGenerator = new(App.GetService<ICodeGenerator>());
    private readonly Lazy<ILoadingService> _loadingService = new(App.GetService<ILoadingService>());
    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>());
    private readonly ISchemaReader _schemaReader;

    public TableItem(Table table, ISchemaReader schemaReader) : base(table.DisplayName, false)
    {
        Table = table;
        _schemaReader = schemaReader;
        GenerateCodeCommand = new AsyncRelayCommand(GenerateCode);
    }

    private async Task GenerateCode()
    {
        var dialog = App.GetService<GenerateCodeDialog>();
        await dialog.ShowAsync();
        if (dialog.ViewModel.DialogResult != ContentDialogResult.Primary) return;

        var templateFolder = dialog.ViewModel.TemplateProjectFolder;
        StorageFolder outputFolder = null;
        try
        {
            if (!Directory.Exists(dialog.ViewModel.OutputPath))
                Directory.CreateDirectory(dialog.ViewModel.OutputPath);
            outputFolder = await StorageFolder.GetFolderFromPathAsync(dialog.ViewModel.OutputPath);
        }
        catch (Exception) {/* ignore */ }
        if (templateFolder == null || outputFolder == null) return;

        _loadingService.Value.Active("Generating...");

#pragma warning disable CS4014
        Task.Run(async () =>
        {
            Table.Columns = _schemaReader.ReadColumns(Table.Database, Table.Schema, Table.Name);
            var files = await _storageService.Value.GetTemplateFilesAsync(templateFolder);
            if (files == null) return;

            foreach (var file in files)
            {
                try
                {
                    var templateText = await FileIO.ReadTextAsync(file);

                    var result = _codeGenerator.Value.GenerateFromTable(Table, templateText);
                    if (result == null) continue;
                    if (string.IsNullOrEmpty(result.Filename))
                    {
                        _notificationService.Value.Error($"Generate template {file.Name} failed: filename not defined.");
                        continue;
                    }

                    var outputFile =
                        await outputFolder.CreateFileAsync(result.Filename, CreationCollisionOption.ReplaceExisting);
                    if (!string.IsNullOrWhiteSpace(result.Codes))
                        await FileIO.WriteTextAsync(outputFile, result.Codes, UnicodeEncoding.Utf8);
                }
                catch (Exception ex)
                {
                    _notificationService.Value.Error($"Generate template {file.Name} failed: {ex.Message}");
                }
            }

            _loadingService.Value.Dismiss();
        });
#pragma warning restore CS4014

    }

}