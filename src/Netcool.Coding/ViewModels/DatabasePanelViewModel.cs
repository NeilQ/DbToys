using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using HandyControl.Controls;
using HandyControl.Data;
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
        private SqlServerLoginView _sqlServiceLoginView;

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
                        _postgreSqlLoginView.ViewModel.Connect
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(builder =>
                        {
                            LoadDatabaseTreeNode(new PostgreSqlSchemaReader(builder), DataBaseType.PostgreSql);
                        });
                        _postgreSqlLoginView.ViewModel.CloseAction = _postgreSqlLoginView.Hide;
                    }
                }
                _postgreSqlLoginView.Show();
            }
            else if (value == "SqlServer")
            {
                if (_sqlServiceLoginView == null)
                {
                    _sqlServiceLoginView = new SqlServerLoginView { Owner = Application.Current.MainWindow };
                    if (_sqlServiceLoginView.ViewModel != null)
                    {
                        _sqlServiceLoginView.ViewModel.Connect
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(builder =>
                        {
                            LoadDatabaseTreeNode(new SqlServerSchemaReader(builder), DataBaseType.PostgreSql);
                        });
                        _sqlServiceLoginView.ViewModel.CloseAction = _sqlServiceLoginView.Hide;
                    }
                }
                _sqlServiceLoginView.Show();
            }
            else if (value == "MySql")
            {

            }
        }

        private void LoadDatabaseTreeNode(ISchemaReader schemaReader, DataBaseType dbType)
        {
            ConnectionItems.Clear();
            var item = new ConnectionItem(schemaReader.GetServerName(), dbType);
            try
            {
                var dbs = schemaReader.ReadDatabases();
                foreach (var db in dbs)
                {
                    item.AddChild(new DatabaseItem(db, schemaReader));
                }

            }
            catch (Exception ex)
            {
                Growl.Error(new GrowlInfo
                {
                    IsCustom = true,
                    Message = $"Read database schema failed: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}",
                    WaitTime = 5
                });
            }
            item.ExpandPath();
            ConnectionItems.AddRange(new[] { item });
        }


    }
}