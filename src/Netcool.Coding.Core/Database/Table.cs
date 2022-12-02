using System;
using System.Collections.Generic;
using System.Linq;

namespace Netcool.Coding.Core.Database
{
    public class Table
    {
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public bool IsView { get; set; }
        public string CleanName { get; set; }
        public string ClassName { get; set; }
        public string SequenceName { get; set; }
        public bool Ignore { get; set; }
        public string Database { get; set; }

        public string DisplayName { get; set; }

        public Column Pk
        {
            get { return Columns?.FirstOrDefault(x => x.IsPk); }
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
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public bool IsPk { get; set; }
        public bool IsNullable { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool Ignore { get; set; }

        public string DbType { get; set; }
        public int? Length { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
    }

}
