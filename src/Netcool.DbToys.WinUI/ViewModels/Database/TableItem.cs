using Netcool.DbToys.Core.Database;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class TableItem : TreeItem
{
    public Table Table { get; set; }

    public TableItem(Table table) : base(table.DisplayName, false)
    {
        Table = table;
    }
}