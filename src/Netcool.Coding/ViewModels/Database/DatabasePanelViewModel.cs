using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using HandyControl.Controls;
using HandyControl.Data;
using Netcool.Coding.Core.Database;
using Netcool.Coding.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Netcool.Coding.ViewModels.Database
{
    public class DatabasePanelViewModel : ReactiveObject
    {
        [Reactive]
        public ObservableCollection<TreeItem> ConnectionItems { get; set; } = new();

        [Reactive]
        public TreeItem SelectedItem { get; set; }

        private PostgreSqlLoginView _postgreSqlLoginView;
        private SqlServerLoginView _sqlServiceLoginView;
        private MySqlLoginView _mySqlLoginView;

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
                                var schemaReader = new PostgreSqlSchemaReader(builder);
                                Locator.CurrentMutable.RegisterConstant<ISchemaReader>(schemaReader);
                                LoadDatabaseTreeNode(schemaReader, DataBaseType.PostgreSql);
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
                            var schemaReader = new SqlServerSchemaReader(builder);
                            Locator.CurrentMutable.RegisterConstant<ISchemaReader>(schemaReader);
                            LoadDatabaseTreeNode(schemaReader, DataBaseType.SqlServer);
                        });
                        _sqlServiceLoginView.ViewModel.CloseAction = _sqlServiceLoginView.Hide;
                    }
                }
                _sqlServiceLoginView.Show();
            }
            else if (value == "MySql")
            {
                if (_mySqlLoginView == null)
                {
                    _mySqlLoginView = new MySqlLoginView { Owner = Application.Current.MainWindow };
                    if (_mySqlLoginView.ViewModel != null)
                    {
                        _mySqlLoginView.ViewModel.Connect
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(builder =>
                            {
                                var schemaReader = new MySqlSchemaReader(builder);
                                Locator.CurrentMutable.RegisterConstant<ISchemaReader>(schemaReader);
                                LoadDatabaseTreeNode(schemaReader, DataBaseType.Mysql);
                            });
                        _mySqlLoginView.ViewModel.CloseAction = _mySqlLoginView.Hide;
                    }
                }
                _mySqlLoginView.Show();
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