using System.Reactive;
using DynamicData.Binding;
using HandyControl.Controls;
using Netcool.Workshop.Database;
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

        public DatabasePanelViewModel()
        {
            var item = new ConnectionItem("first", new[] { new ConnectionItem("second", new[] { new ConnectionItem("three") }) });
            item.CollapsePath();
            ConnectionItems.Load(new[] { item });

            NewConnectionCommand = ReactiveCommand.Create<string>((value) =>
            {
                Growl.Info(value);
            });
        }

    }
}