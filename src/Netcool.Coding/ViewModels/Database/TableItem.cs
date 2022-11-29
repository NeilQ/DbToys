using System;
using System.Collections.Generic;
using Netcool.Coding.Events;
using ReactiveUI;

namespace Netcool.Coding.ViewModels.Database;

public class TableItem : TreeItem
{
    public string Database { get; set; }
    public string TableName { get; set; }
    public TableItem(string name, string tableName, string database, IEnumerable<TreeItem> children = null) : base(name, false)
    {
        TableName = tableName;
        Database = database;
    }

    protected override void OnSelected()
    {
        MessageBus.Current.SendMessage(new TableSelectedEvent(Database, TableName));
    }
}