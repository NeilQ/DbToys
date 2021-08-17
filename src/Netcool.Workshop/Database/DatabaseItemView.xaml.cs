using System.Reactive.Disposables;
using ReactiveUI;

namespace Netcool.Workshop.Database
{
    /// <summary>
    /// Interaction logic for DatabaseItemView.xaml
    /// </summary>
    public partial class DatabaseItemView
    {
        public DatabaseItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d);
            });
        }
    }
}
