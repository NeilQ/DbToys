namespace Netcool.Coding.Events;

public class TableSelectedEvent
{
    public string TableName { get; set; }
    public string Database { get; set; }

    public TableSelectedEvent(string database, string tableName)
    {
        Database = database;
        TableName = tableName;
    }
}