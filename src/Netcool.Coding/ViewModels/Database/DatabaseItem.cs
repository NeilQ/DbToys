using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Netcool.Coding.Core.Database;
using ReactiveUI;

namespace Netcool.Coding.ViewModels.Database
{
    public class DatabaseItem : TreeItem
    {

        private readonly ISchemaReader _schemaReader;

        public List<Table> Tables { get; set; }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExportToExcelCommand { get; set; }

        public DatabaseItem(string name, ISchemaReader schemaReader) : base(name, true)
        {
            Name = name;
            _schemaReader = schemaReader;
            RefreshCommand = ReactiveCommand.Create(LoadChildren);
            ExportToExcelCommand = ReactiveCommand.CreateFromTask(ExportToExcel);
        }

        private Task<Unit> ExportToExcel()
        {
            throw new System.NotImplementedException();
        }

        protected override void LoadChildren()
        {
            var tables = _schemaReader.ReadTables(Name).OrderBy(t => t.DisplayName);
            Tables = tables.ToList();
            Children.Clear();
            foreach (var table in Tables)
            {
                AddChild(new TableItem(table.DisplayName, table.Name, Name) { IsExpanded = true });
            }
        }
    }
}