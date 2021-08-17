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


namespace Netcool.Workshop.ViewModels
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public IObservableCollection<TreeItem> ConnectionItems { get; set; } = new ObservableCollectionExtended<TreeItem>();

        [Reactive]
        public TreeItem SelectedItem { get; set; }

        public ReactiveCommand<string, Unit> NewConnectionCommand { get; set; }

        private PostgreSqlLoginViewModel _postgreSqlLoginViewModel;

        public DatabasePanelViewModel()
        {
            NewConnectionCommand = ReactiveCommand.Create<string>(OpenConnectWindow);
        }

        private void OpenConnectWindow(string value)
        {
            Growl.Info(value);

            if (value == "PostgreSql")
            {
                _postgreSqlLoginViewModel ??= new PostgreSqlLoginViewModel();
                var window = new PostgreSqlLoginView { Owner = Application.Current.MainWindow, ViewModel = _postgreSqlLoginViewModel };
                _postgreSqlLoginViewModel.Cancel.Subscribe(_ => { window.Close(); });
                _postgreSqlLoginViewModel.Connect.Subscribe(builder =>
                {
                    window.Close();
                    LoadDatabaseTreeNode(new PostgreSqlSchemaReader(builder), DataBaseType.PostgreSql);
                });

                window.Show();
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