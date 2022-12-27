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
using Netcool.DbToys.WinUI.Views.Database;

namespace Netcool.DbToys.WinUI.ViewModels;

public class DatabaseViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;
    public IAsyncRelayCommand<string> ConnectCommand { get; }

    public ISchemaReader SchemaReader { get; private set; }

    public ObservableCollection<object> TableResultSet { get; set; } = new();

    public ObservableCollection<Column> TableColumns { get; set; } = new();

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

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedItem))
        {
            if (SelectedItem is TableItem tableItem)
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
                    await _notificationService.QueueNotificationAsync(new Notification("Read column info failed",
                          ex.InnerException == null ? ex.Message : ex.InnerException.Message, InfoBarSeverity.Error, 5000));
                }

                DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
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
                LoadDatabaseTreeNode(SchemaReader, DataBaseType.PostgreSql);
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
                LoadDatabaseTreeNode(SchemaReader, DataBaseType.Mysql);
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
                LoadDatabaseTreeNode(SchemaReader, DataBaseType.SqlServer);
            }
        }
    }

    private void LoadDatabaseTreeNode(ISchemaReader schemaReader, DataBaseType dbType)
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
            _notificationService.QueueNotificationAsync(new Notification("Read database schema failed",
                ex.InnerException == null ? ex.Message : ex.InnerException.Message, InfoBarSeverity.Error, 5000));
        }
        item.ExpandPath();
        ConnectionItems.Add(item);
    }
}