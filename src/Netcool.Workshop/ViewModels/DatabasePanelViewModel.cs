using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reactive;
using System.Windows;
using DynamicData.Binding;
using HandyControl.Controls;
using Netcool.Workshop.Core.Database;
using Netcool.Workshop.Database;
using Netcool.Workshop.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Window = System.Windows.Window;


namespace Netcool.Workshop.ViewModels
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public IObservableCollection<TreeItem> ConnectionItems { get; set; } = new ObservableCollectionExtended<TreeItem>();

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
            Growl.Info(value);

            if (_loginWindow != null) return;
            if (value == "PostgreSql")
            {
                {
                    var window = new PostgreSqlLoginView()
                    { ViewModel = new PostgreSqlLoginViewModel(), Owner = Application.Current.MainWindow };
                    window.Closed += (_, _) => { _loginWindow = null; };
                    _loginWindow = window;
                    window.ViewModel?.Connect.Subscribe(builder =>
                    {
                        LoadDatabaseTreeNode(new PostgreSqlSchemaReader(builder), DataBaseType.PostgreSql);
                    });

                    window.Show();
                }
            }
        }

        private void LoadDatabaseTreeNode(ISchemaReader schemaReader, DataBaseType dbType)
        {
            var item = new ConnectionItem(schemaReader.GetServerName(), dbType, new List<TreeItem>());
            var dbs = schemaReader.ReadDatabases();
            foreach (var db in dbs)
            {
                item.AddChild(new DatabaseItem(db, schemaReader));
            }
            item.ExpandPath();
            ConnectionItems.Load(new[] { item });
        }


    }
}