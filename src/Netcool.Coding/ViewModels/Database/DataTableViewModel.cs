using ReactiveUI;
using Splat;

namespace Netcool.Coding.ViewModels;

public class DataTableViewModel : ReactiveObject, IRoutableViewModel
{
    public DataTableViewModel(IScreen screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>();
    }
    public string UrlPathSegment => "data-table";
    public IScreen HostScreen { get; }
}