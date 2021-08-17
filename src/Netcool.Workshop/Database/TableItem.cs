using System.Collections.Generic;
using Netcool.Workshop.ViewModels;

namespace Netcool.Workshop.Database
{
    public class TableItem : TreeItem
    {
        public TableItem(string name, IEnumerable<TreeItem> children = null) : base(name, children)
        {
        }

        public override object ViewModel => this;
    }
}