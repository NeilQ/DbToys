using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Workshop.ViewModels
{
    public abstract class TreeItem : ReactiveObject, IActivatableViewModel
    {
        public abstract object ViewModel { get; }

        public ViewModelActivator Activator { get; }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public IObservableCollection<TreeItem> Children { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (!value && !Children.Any())
                {
                    return;
                }
                this.RaiseAndSetIfChanged(ref _isExpanded, value);
            }
        }

        [Reactive]
        public bool IsSelected { get; set; }


        private TreeItem _parent;

        protected TreeItem(string name, IEnumerable<TreeItem> children = null)
        {
            Name = name;
            Activator = new ViewModelActivator();
            Children = new ObservableCollectionExtended<TreeItem>();
            if (children == null) return;
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