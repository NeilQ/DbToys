using Netcool.Coding.Core.Database;
using Netcool.Coding.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.Database
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