using ReactiveUI;
using Splat;

namespace Netcool.Workshop.ViewModels
{
    public class LayoutViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "layout";
        public IScreen HostScreen { get; }

        public LayoutViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }

    }
}