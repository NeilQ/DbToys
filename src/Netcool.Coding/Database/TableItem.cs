using System.Collections.Generic;
using Netcool.Coding.ViewModels;

namespace Netcool.Coding.Database
{
    public class TableItem : TreeItem
    {
        public TableItem(string name, IEnumerable<TreeItem> children = null) : base(name, false)
        {
        }
    }
}