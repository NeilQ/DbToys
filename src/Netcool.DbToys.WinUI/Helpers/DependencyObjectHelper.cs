using System.Reflection;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace Netcool.DbToys.WinUI.Helpers;

public static class DependencyObjectHelper
{
    public static T FindChild<T>(DependencyObject startNode) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(startNode);
        for (int i = 0; i < count; i++)
        {
            DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
            if (current.GetType() == typeof(T) || current.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
            {
                T asType = (T)current;
                return asType;
            }
            var retVal = FindChild<T>(current);
            if (retVal is not null)
            {
                return retVal;
            }
        }
        return null;
    }

    public static T FindChild<T>(DependencyObject startNode, Func<T, bool> predicate) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(startNode);
        for (int i = 0; i < count; i++)
        {
            DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
            if (current.GetType() == typeof(T) || current.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
            {
                T asType = (T)current;
                if (predicate(asType))
                {
                    return asType;
                }
            }
            var retVal = FindChild(current, predicate);
            if (retVal is not null)
            {
                return retVal;
            }
        }
        return null;
    }

    public static IEnumerable<T> FindChildren<T>(DependencyObject startNode) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(startNode);
        for (int i = 0; i < count; i++)
        {
            var current = VisualTreeHelper.GetChild(startNode, i);
            if (current.GetType() == typeof(T) || current.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
            {
                T asType = (T)current;
                yield return asType;
            }
            foreach (var item in FindChildren<T>(current))
            {
                yield return item;
            }
        }
    }

    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        if (child is null) return null;
        T parent = null;

        var currentParent = VisualTreeHelper.GetParent(child);
        while (currentParent is not null)
        {
            if (currentParent is T dependencyObject)
            {
                parent = dependencyObject;
                break;
            }
            currentParent = VisualTreeHelper.GetParent(currentParent);
        }
        return parent;
    }
}