using System.Collections.Generic;
using Netcool.Workshop.Core.Database;
using Netcool.Workshop.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Workshop.Database
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