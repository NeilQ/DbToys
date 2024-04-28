using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using DbToys.Core.Database;
using DbToys.Services;
using Microsoft.UI.Dispatching;

namespace DbToys.ViewModels;

public class TableDetailViewModel : ObservableRecipient
{
    public Table SelectedTable { get; set; }

    public ObservableCollection<object> TableResultSet { get; set; } = new();

    public ObservableCollection<Column> TableColumns { get; set; } = new();

    public ISchemaReader SchemaReader { get; set; }

    public Action<DataColumnCollection> OnResultSetLoaded;

    private readonly INotificationService _notificationService;

    public TableDetailViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        InitData();
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        TableResultSet = null;
        SchemaReader = null;
        TableColumns = null;
        SelectedTable = null;
        OnResultSetLoaded = null;
    }

    private void InitData()
    {
        var table = SelectedTable;
        if (table == null) return;

        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Task.Run(() =>
        {
            List<Column> columns = null;
            DataTable resultSet = null;
            try
            {
                columns = SchemaReader?.ReadColumns(table.Database, table.Schema,
                    table.Name);
                var firstPk = columns?.FirstOrDefault(t => t.IsPk);
                var sort = firstPk == null ? null : firstPk.Name + " desc";
                resultSet = SchemaReader?.GetResultSet(table, 30, sort);
            }
            catch (Exception ex)
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    _notificationService.Error(ex.Message, "Read column info failed");
                });
            }

            dispatcherQueue.TryEnqueue(() =>
            {
                TableColumns.Clear();
                TableResultSet?.Clear();
                columns?.ForEach(item => { TableColumns.Add(item); });
                if (resultSet != null)
                {
                    OnResultSetLoaded(resultSet.Columns);
                    foreach (DataRow row in resultSet.Rows)
                    {
                        TableResultSet?.Add(row.ItemArray);
                    }
                }
            });
        });
    }
}