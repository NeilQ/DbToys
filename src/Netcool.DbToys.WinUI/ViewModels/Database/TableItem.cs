using Netcool.DbToys.Core.Database;

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

    public TableItem(Table table) : base(table.DisplayName, false)
    {
        Table = table;
    }
}