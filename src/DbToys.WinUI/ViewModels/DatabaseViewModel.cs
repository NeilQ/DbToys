using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbToys.Core.Database;
using DbToys.Services;
using DbToys.ViewModels.Database;
using DbToys.Views.Dialogs;

namespace DbToys.ViewModels;

public class DatabaseViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;

    public IAsyncRelayCommand<string> ConnectCommand { get; }

    public ISchemaReader SchemaReader { get; private set; }

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
        // issue: may case crash https://github.com/microsoft/microsoft-ui-xaml/issues/2121
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
            _notificationService.Error(ex.Message, "Read database schema failed");
        }
        ConnectionItems.Add(item);
        item.ExpandPath();
    }
}