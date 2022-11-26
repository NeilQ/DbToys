using ReactiveUI;
using Splat;

namespace Netcool.Coding.ViewModels;

public class DatabaseLayoutViewModel:  ReactiveObject, IRoutableViewModel
{
    public string UrlPathSegment => "database";
    public IScreen HostScreen { get; }

    public DatabaseLayoutViewModel(IScreen screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>();
    }
}