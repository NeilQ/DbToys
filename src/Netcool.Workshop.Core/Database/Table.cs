using System;
using System.Collections.Generic;
using System.Linq;

namespace Netcool.Workshop.Core.Database
{
    public class Table
    {
        public List<Column> Columns;
        public string Name;
        public string Schema;
        public bool IsView;
        public string CleanName;
        public string ClassName;
        public string SequenceName;
        public bool Ignore;

        public string DisplayName => $"{Schema}.{Name}";

        public Column Pk
        {
            get { return Columns.SingleOrDefault(x => x.IsPk); }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public Column this[string columnName] => GetColumn(columnName);

        public override string ToString()
        {
            return (Schema ?? "default") + "." + Name;
        }
    }

    public class Column
    {
        public string Name;
        public string PropertyName;
        public string PropertyType;
        public bool IsPk;
        public bool IsNullable;
        public bool IsAutoIncrement;
        public bool Ignore;

        public string DbType { get; set; }
        public int? Length { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
    }

    public class TableCollection : List<Table>
    {
        public TableCollection()
        {
        }

        public Table GetTable(string tableName)
        {
            return this.Single(x => string.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public Table this[string tableName] => GetTable(tableName);
    }
}
