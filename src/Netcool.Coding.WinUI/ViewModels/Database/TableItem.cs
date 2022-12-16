using Netcool.Coding.Core.Database;

namespace Netcool.Coding.WinUI.ViewModels.Database;

public class TableItem : TreeItem
{
    public Table Table { get; set; }

    public TableItem(Table table) : base(table.DisplayName, false)
    {
        Table = table;
    }
}