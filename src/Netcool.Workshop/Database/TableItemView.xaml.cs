using System.Reactive.Disposables;
using ReactiveUI;

namespace Netcool.Workshop.Database
{
    /// <summary>
    /// Interaction logic for TableItemView.xaml
    /// </summary>
    public partial class TableItemView
    {
        public TableItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d);
            });
        }
    }
}
