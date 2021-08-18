using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using DynamicData;
using HandyControl.Controls;
using Netcool.Workshop.Core.Database;
using Netcool.Workshop.Database;
using Netcool.Workshop.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Window = System.Windows.Window;


namespace Netcool.Workshop.ViewModels
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public ObservableCollection<TreeItem> ConnectionItems { get; set; } = new ObservableCollection<TreeItem>();

        [Reactive]
        public TreeItem SelectedItem { get; set; }

        private Window _loginWindow;

        public ReactiveCommand<string, Unit> NewConnectionCommand { get; set; }

        public DatabasePanelViewModel()
        {
            NewConnectionCommand = ReactiveCommand.Create<string>(OpenConnectWindow);
        }

        private void OpenConnectWindow(string value)
        {
            if (_loginWindow != null) return;
            if (value == "PostgreSql")
            {
                var window = new PostgreSqlLoginView() { Owner = Application.Current.MainWindow };
                _loginWindow = window;
                 window.ViewModel?.Connect.Subscribe(builder =>
                {
                    LoadDatabaseTreeNode(new PostgreSqlSchemaReader(builder), DataBaseType.PostgreSql);
                });
                window.Closed += (_, _) =>
                {
                    _loginWindow = null;
                };

                window.Show();
            }
        }

        private void LoadDatabaseTreeNode(ISchemaReader schemaReader, DataBaseType dbType)
        {
            var item = new ConnectionItem(schemaReader.GetServerName(), dbType);
            var dbs = schemaReader.ReadDatabases();
            foreach (var db in dbs)
            {
                item.AddChild(new DatabaseItem(db, schemaReader));
            }
            item.ExpandPath();
            ConnectionItems.Clear();
            ConnectionItems.AddRange(new[] { item });
        }


    }
}