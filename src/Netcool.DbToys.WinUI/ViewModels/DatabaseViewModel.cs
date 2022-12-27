using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Services;
using Netcool.DbToys.WinUI.ViewModels.Database;
using Netcool.DbToys.WinUI.Views.Database;

namespace Netcool.DbToys.WinUI.ViewModels;

public class DatabaseViewModel : ObservableRecipient
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
        ConnectCommand = new AsyncRelayCommand<string>(async dbType =>
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
        });
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
                ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            /*
            Growl.Error(new GrowlInfo
            {
                IsCustom = true,
                Message = $"Read database schema failed: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}",
                WaitTime = 5
            });
            */
        }
        item.ExpandPath();
        ConnectionItems.Add(item);
    }
}