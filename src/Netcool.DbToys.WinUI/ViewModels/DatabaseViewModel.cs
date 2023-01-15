using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Services;
using Netcool.DbToys.WinUI.ViewModels.Database;
using Netcool.DbToys.WinUI.Views.Dialogs;

namespace Netcool.DbToys.WinUI.ViewModels;

public class DatabaseViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;

    public IAsyncRelayCommand<string> ConnectCommand { get; }

    public ISchemaReader SchemaReader { get; private set; }

    public ObservableCollection<object> TableResultSet { get; set; } = new();

    public ObservableCollection<Column> TableColumns { get; set; } = new();

    private bool _loadingTable;
    public bool LoadingTable
    {
        get => _loadingTable;
        set => SetProperty(ref _loadingTable, value);
    }

    private ObservableCollection<TreeItem> _connectionItems = new();
    public ObservableCollection<TreeItem> ConnectionItems
    {
        get => _connectionItems;
        set => SetProperty(ref _connectionItems, value);
    }

    private object _selectedItem;
    public object SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public DatabaseViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
        ConnectCommand = new AsyncRelayCommand<string>(ConnectDatabase);
        PropertyChanged += OnPropertyChanged;
    }

    public Action<DataColumnCollection> OnResultSetLoaded;

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedItem))
        {
            if (SelectedItem is TableItem tableItem)
            {
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                dispatcherQueue.TryEnqueue(() =>
                {
                    tableItem.LoadingColumns = true;
                });
                Task.Run(() =>
                {
                    List<Column> columns = null;
                    DataTable resultSet = null;
                    try
                    {
                        columns = SchemaReader?.ReadColumns(tableItem.Table.Database, tableItem.Table.Schema, tableItem.Table.Name);
                        tableItem.Table.Columns = columns;
                        resultSet = SchemaReader?.GetResultSet(tableItem.Table, 30);
                    }
                    catch (Exception ex)
                    {
                        _notificationService.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message,
                            "Read column info failed");
                    }

                    dispatcherQueue.TryEnqueue(() =>
                    {
                        TableColumns.Clear();
                        TableResultSet?.Clear();
                        columns?.ForEach(item =>
                        {
                            TableColumns.Add(item);
                        });
                        if (resultSet != null)
                        {
                            OnResultSetLoaded(resultSet.Columns);
                            foreach (DataRow row in resultSet.Rows)
                            {
                                TableResultSet?.Add(row.ItemArray);
                            }
                        }

                        tableItem.LoadingColumns = false;
                    });
                });
            }
        }
    }


    private async Task ConnectDatabase(string dbType)
    {
        if (dbType == "PostgreSql")
        {
            var dialog = App.GetService<PostgreSqlConnectDialog>();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await dialog.ShowAsync();
            if (dialog.ViewModel.SchemaReader != null)
            {
                SchemaReader = dialog.ViewModel.SchemaReader;
                LoadDatabaseTreeNode(SchemaReader, DatabaseType.PostgreSql);
            }
        }
        else if (dbType == "MySql")
        {
            var dialog = App.GetService<MysqlConnectDialog>();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await dialog.ShowAsync();
            if (dialog.ViewModel.SchemaReader != null)
            {
                SchemaReader = dialog.ViewModel.SchemaReader;
                LoadDatabaseTreeNode(SchemaReader, DatabaseType.Mysql);
            }
        }
        else if (dbType == "SqlServer")
        {
            var dialog = App.GetService<SqlServerConnectDialog>();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await dialog.ShowAsync();
            if (dialog.ViewModel.SchemaReader != null)
            {
                SchemaReader = dialog.ViewModel.SchemaReader;
                LoadDatabaseTreeNode(SchemaReader, DatabaseType.SqlServer);
            }
        }
    }

    private void LoadDatabaseTreeNode(ISchemaReader schemaReader, DatabaseType dbType)
    {
        ConnectionItems.Clear();
        var item = new ConnectionItem(schemaReader.GetServerName(), dbType);
        try
        {
            var dbs = schemaReader.ReadDatabases();
            foreach (var db in dbs)
            {
                item.AddChild(new DatabaseItem(db, schemaReader));
            }
        }
        catch (Exception ex)
        {
            _notificationService.QueueNotification(new Notification("Read database schema failed",
                ex.InnerException == null ? ex.Message : ex.InnerException.Message, InfoBarSeverity.Error, 5000));
        }
        ConnectionItems.Add(item);
        item.ExpandPath();
    }
}