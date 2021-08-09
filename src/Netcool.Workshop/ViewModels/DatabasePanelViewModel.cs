using System.Reactive;
using System.Windows;
using DynamicData.Binding;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Netcool.Workshop.Database;
using Netcool.Workshop.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Netcool.Workshop.ViewModels
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public IObservableCollection<TreeItem> ConnectionItems { get; set; } = new ObservableCollectionExtended<TreeItem>();

        [Reactive]
        public TreeItem SelectedItem { get; set; }

        public ReactiveCommand<string, Unit> NewConnectionCommand { get; set; }

        private PostgreSqlLoginViewModel _postgreSqlLoginViewModel = new PostgreSqlLoginViewModel();

        public DatabasePanelViewModel()
        {
            var item = new ConnectionItem("first", new[] { new ConnectionItem("second", new[] { new ConnectionItem("three") }) });
            item.CollapsePath();
            ConnectionItems.Load(new[] { item });

            NewConnectionCommand = ReactiveCommand.Create<string>((value) =>
            {
                Growl.Info(value);

                if (value == "PostgreSql")
                {
                    var window = new PostgreSqlLoginView
                    {
                        Owner = Application.Current.MainWindow,
                        ViewModel = _postgreSqlLoginViewModel
                    };
                    _postgreSqlLoginViewModel.CloseAction = () => { window.Close(); };
                    window.Show();
                }
            });
        }

    }
}