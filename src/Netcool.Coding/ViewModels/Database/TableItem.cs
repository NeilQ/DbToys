using System.Threading.Tasks;
using Netcool.Coding.Core.Database;
using Netcool.Coding.Events;
using ReactiveUI;

namespace Netcool.Coding.ViewModels.Database;

public class TableItem : TreeItem
{
    public Table Table { get; set; }
    public TableItem(Table table) : base(table.DisplayName, false)
    {
        Table = table;
    }

    protected override void OnSelected()
    {
        Task.Run(() =>
        {
            MessageBus.Current.SendMessage(new TableSelectedEvent(Table));
        });
    }
}