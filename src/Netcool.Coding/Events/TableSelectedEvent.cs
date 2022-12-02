using Netcool.Coding.Core.Database;

namespace Netcool.Coding.Events;

public class TableSelectedEvent
{
    public Table Table { get; set; }

    public TableSelectedEvent(Table table)
    {
        Table = table;
    }
}