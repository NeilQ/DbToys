using System.Reactive.Disposables;
using ReactiveUI;

namespace Netcool.Workshop.Database
{
    /// <summary>
    /// Interaction logic for ConnectionItemView.xaml
    /// </summary>
    public partial class ConnectionItemView
    {
        public ConnectionItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.ConnectionName.Text).DisposeWith(d);
            });
        }
    }
}
