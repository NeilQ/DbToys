﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DbToys.ViewModels;


public class TreeItem : ObservableRecipient
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ObservableCollection<TreeItem> Children { get; set; }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            SetProperty(ref _isExpanded, value);
            // Lazy load the child items, if necessary.
            if (_isExpanded && HasUnrealizedChildren && Children.Count == 0)
            {
                LoadChildren();
            }
        }
    }

    protected virtual void LoadChildren()
    {
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    private bool _hasUnrealizedChildren;
    public bool HasUnrealizedChildren
    {
        get => _hasUnrealizedChildren;
        set => SetProperty(ref _hasUnrealizedChildren, value);
    }

    private TreeItem _parent;

    protected TreeItem(string name, bool lazyLoadChildren)
    {
        _name = name;
        Children = new ObservableCollection<TreeItem>();
        if (lazyLoadChildren) _hasUnrealizedChildren = true;
    }

    protected TreeItem(string name)

    {
        _name = name;
    }

    public void AddChild(TreeItem child)
    {
        child._parent = this;
        Children!.Add(child);
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