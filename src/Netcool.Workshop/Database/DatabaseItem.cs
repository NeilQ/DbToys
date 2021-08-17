using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Netcool.Workshop.Core.Database;
using Netcool.Workshop.ViewModels;
using ReactiveUI;

namespace Netcool.Workshop.Database
{
    public class DatabaseItem : TreeItem
    {
        public override object ViewModel => this;

        private readonly ISchemaReader _schemaReader;

        public TableCollection Tables { get; set; }

        public DatabaseItem(string name, ISchemaReader schemaReader, IEnumerable<TreeItem> children = null) : base(name, children)
        {
            Name = name;
            _schemaReader = schemaReader;

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(t => t.IsExpanded)
                    .Where(t => t)
                    .Subscribe(_ =>
                    {
                        if (!Children.Any())
                        {
                            LoadChildren();
                        }
                    })
                    .DisposeWith(d);
            });
        }

        private void LoadChildren()
        {
            var tables = _schemaReader.ReadTables(Name);
            Tables = tables;
            Children.Load(tables.Select(t => new TableItem(t.DisplayName)));
        }
    }
}