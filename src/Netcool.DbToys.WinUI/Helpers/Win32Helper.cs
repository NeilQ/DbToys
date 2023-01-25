using System.Runtime.InteropServices;

namespace Netcool.DbToys.Helpers;

public static class Win32Helper
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
}