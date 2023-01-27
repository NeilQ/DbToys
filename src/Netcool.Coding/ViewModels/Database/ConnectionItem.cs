using Netcool.DbToys.Core.Database;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels.Database
{
    public class ConnectionItem : TreeItem
    {
        [Reactive]
        public DatabaseType DatabaseType { get; set; }

        public ConnectionItem(string name, DatabaseType databaseType) : base(name, false)
        {
            DatabaseType = databaseType;
        }

    }
}