using ReactiveUI;

namespace Netcool.Workshop.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router
        {
            get;
        }
        public MainWindowViewModel()
        {
            Router = new RoutingState();

            RegisterDependencies();
        }

        public void RegisterDependencies()
        {

        }

    }
}
