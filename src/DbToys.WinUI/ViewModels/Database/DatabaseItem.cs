using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.Input;
using DbToys.Core.Database;
using DbToys.Core.Excel;
using DbToys.Helpers;
using DbToys.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace DbToys.ViewModels.Database;

public class DatabaseItem : TreeItem
{
    private readonly Lazy<IExcelService> _excelService = new(App.GetService<IExcelService>);
    private readonly Lazy<ILoadingService> _loadingService = new(App.GetService<ILoadingService>);
    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>);
    private readonly ISchemaReader _schemaReader;

    public IRelayCommand RefreshCommand { get; }

    public IRelayCommand ExportCommand { get; }

    public List<Table> Tables { get; set; }

    private bool _expanding;
    public bool Expanding
    {
        get => _expanding;
        set => SetProperty(ref _expanding, value);
    }

    public DatabaseItem(string name, ISchemaReader schemaReader) : base(name, true)
    {
        Name = name;
        _schemaReader = schemaReader;
        RefreshCommand = new RelayCommand(LoadChildren);
        ExportCommand = new AsyncRelayCommand(ExportAsync);
    }

    protected override void LoadChildren()
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        dispatcherQueue.TryEnqueue(() =>
        {
            Expanding = true;
        });

        Task.Run(() =>
        {
            try
            {
                var tables = _schemaReader.ReadTables(Name).OrderBy(t => t.DisplayName).ToList();
                Tables = tables.ToList();
            }
            catch (Exception ex)
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    Expanding = true;
                    _notificationService.Value.Error($"Read table schema failed: {ex.Message}");
                });
            }

            dispatcherQueue.TryEnqueue(() =>
            {
                Children?.Clear();
                if (Tables != null)
                {
                    foreach (var table in Tables)
                    {
                        AddChild(new TableItem(table, _schemaReader));
                    }
                }
                HasUnrealizedChildren = false;
                Expanding = false;
            });
        });

    }
    private async Task ExportAsync()
    {
        var savePicker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.Desktop, SuggestedFileName = $"{Name} Data Dictionary", DefaultFileExtension = ".xlsx" };
        savePicker.FileTypeChoices.Add("Excel files", new List<string> { ".xlsx", ".xls" });

        // When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
        if (Window.Current == null)
        {
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, Win32Helper.GetActiveWindow());
        }

        var file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            _loadingService.Value.Active("Exporting data dictionary...");
            await Task.Run(() =>
            {
                try
                {
                    var tables = _schemaReader.ReadTables(Name).OrderBy(t => t.DisplayName).ToList();
                    foreach (var table in tables)
                    {
                        table.Columns = _schemaReader.ReadColumns(table.Database, table.Schema, table.Name);
                    }

                    _excelService.Value.GenerateDatabaseDictionary(tables, file.Path);
                    _notificationService.Value.Success($"{file.Name} saved");
                }
                catch (Exception ex)
                {
                    _notificationService.Value.Error(ex.Message, "Exporting failed");
                }
                finally
                {
                    _loadingService.Value.Dismiss();
                }

                return Task.CompletedTask;
            });
        }
    }
}
