// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.ViewModels;
using Netcool.DbToys.WinUI.ViewModels.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Netcool.DbToys.WinUI.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DatabasePage : Page
{
    private readonly InputCursor _arrowCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 0));

    private readonly InputCursor _resizeCursor =
        InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.SizeWestEast, 1));

    public DatabaseViewModel ViewModel { get; }

    public DatabasePage()
    {
        ViewModel=App.GetService<DatabaseViewModel>();
        InitializeComponent();
    }

    private void GridSplitter_OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        this.ChangeCursor(_resizeCursor);
    }

    private void GridSplitter_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        this.ChangeCursor(_arrowCursor);
    }

    private void GridSplitter_OnLoaded(object sender, RoutedEventArgs e)
    {
        (sender as UIElement)?.ChangeCursor(_resizeCursor);
    }
}

public class DatabaseTreeItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ConnectionTemplate { get; set; }
    public DataTemplate DatabaseTemplate { get; set; }
    public DataTemplate TableTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {

        //return FileTemplate;
        if (item is ConnectionItem) return ConnectionTemplate;
        if (item is DatabaseItem) return DatabaseTemplate;
        if (item is TableItem) return TableTemplate;
        return ConnectionTemplate;
    }
}
