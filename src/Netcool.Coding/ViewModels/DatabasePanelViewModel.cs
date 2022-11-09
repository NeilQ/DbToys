using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows;
using DynamicData;
using Netcool.Coding.Core.Database;
using Netcool.Coding.Database;
using Netcool.Coding.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public ObservableCollection<TreeItem> ConnectionItems { get; set; } = new();

        [Reactive]
        public TreeItem SelectedItem { get; set; }

        private PostgreSqlLoginView _postgreSqlLoginView;

        public ReactiveCommand<string, Unit> NewConnectionCommand { get; set; }

        public DatabasePanelViewModel()
        {
            NewConnectionCommand = ReactiveCommand.Create<string>(OpenConnectWindow);

        }

        private void OpenConnectWindow(string value)
        {
            if (value == "PostgreSql")
            {
                if (_postgreSqlLoginView == null)
                {
                    _postgreSqlLoginView = new PostgreSqlLoginView { Owner = Application.Current.MainWindow };

                    if (_postgreSqlLoginView.ViewModel != null)
                    {
                        _postgreSqlLoginView.ViewModel.Connect.Subscribe(builder =>
                        {
                            LoadDatabaseTreeNode(new PostgreSqlSchemaReader(builder), DataBaseType.PostgreSql);
                        });
                        _postgreSqlLoginView.ViewModel.CloseAction = () =>
                        {
                            _postgreSqlLoginView.Hide();
                        };
                    }
                }
                _postgreSqlLoginView.Show();
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