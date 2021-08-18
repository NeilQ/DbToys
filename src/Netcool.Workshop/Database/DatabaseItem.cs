using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Netcool.Workshop.Core.Database;
using Netcool.Workshop.ViewModels;
using ReactiveUI;

namespace Netcool.Workshop.Database
{
    public class DatabaseItem : TreeItem
    {

        private readonly ISchemaReader _schemaReader;

        public TableCollection Tables { get; set; }

        public DatabaseItem(string name, ISchemaReader schemaReader) : base(name, true)
        {
            Name = name;
            _schemaReader = schemaReader;
        }

        protected override void LoadChildren()
        {
            var tables = _schemaReader.ReadTables(Name);
            Tables = tables;
            Children.Clear();
            foreach (var table in Tables)
            {
                AddChild(new TableItem(table.DisplayName) { IsExpanded = true });
            }
        }
    }
}