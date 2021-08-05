using System.Collections.Generic;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Workshop.ViewModels
{
    public abstract class TreeItem : ReactiveObject
    {
        public abstract object ViewModel { get; }

        [Reactive]
        public IObservableCollection<TreeItem> Children { get; set; }

        [Reactive]
        public bool IsExpanded { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }


        private TreeItem _parent;

        protected TreeItem(IEnumerable<TreeItem> children = null)
        {
            Children = new ObservableCollectionExtended<TreeItem>();
            if (children == null) return;
            //Children.Load(children);
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public void AddChild(TreeItem child)
        {
            child._parent = this;
            Children.Add(child);
        }

        public void ExpandPath()
        {
            IsExpanded = true;
            _parent?.ExpandPath();
        }

        public void CollapsePath()
        {
            IsExpanded = false;
            _parent?.CollapsePath();
        }
    }
}