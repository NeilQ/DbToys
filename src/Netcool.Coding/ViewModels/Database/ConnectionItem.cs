using Netcool.DbToys.Core.Database;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels.Database
{
    public class ConnectionItem : TreeItem
    {
        [Reactive]
        public DataBaseType DataBaseType { get; set; }

        public ConnectionItem(string name, DataBaseType databaseType) : base(name, false)
        {
            DataBaseType = databaseType;
        }

    }
}