using ReactiveUI;
using Splat;

namespace Netcool.Coding.ViewModels
{
    public class LayoutViewModel : ReactiveObject,IScreen
    {
        public RoutingState Router { get; }

        public LayoutViewModel()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
        }

    }
}