using System.Collections.Generic;
using Netcool.Workshop.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Workshop.Database
{
    public class ConnectionItem : TreeItem
    {
        [Reactive]
        public string Name { get; set; }

        public override object ViewModel => this;

        public ConnectionItem(string name, IEnumerable<TreeItem> children = null) : base(children)
        {
            Name = name;
        }

        public ConnectionItem() { }
    }
}