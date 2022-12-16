using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;

namespace Netcool.Coding.WinUI.Helpers;

public static class UIElementExtensions
{
    public static void ChangeCursor(this UIElement uiElement, InputCursor cursor)
    {
        Type type = typeof(UIElement);
        type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
    }
}