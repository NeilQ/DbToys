using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels
{
    public  class TreeItem : ReactiveObject
    {
        private static readonly TreeItem DummyChild = new();

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public ObservableCollection<TreeItem> Children { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                this.RaiseAndSetIfChanged(ref _isExpanded, value);
                // Lazy load the child items, if necessary.
                if (HasDummyChild)
                {
                    Children.Remove(DummyChild);
                    LoadChildren();
                }
            }
        }

        protected virtual void LoadChildren()
        {
        }

        [Reactive]
        public bool IsSelected { get; set; }

        public bool HasDummyChild => Children.Count == 1 && Children[0] == DummyChild;


        private TreeItem _parent;

        protected TreeItem(string name, bool lazyLoadChildren)
        {
            Name = name;
            Children = new ObservableCollection<TreeItem>();
            if (lazyLoadChildren) Children.Add(DummyChild);
        }

        private TreeItem()
        {
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