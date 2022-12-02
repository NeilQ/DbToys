using System;
using System.Collections.Generic;
using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Documents;
using DynamicData.Binding;
using HandyControl.Controls;
using HandyControl.Data;
using Netcool.Coding.Core.Database;
using Netcool.Coding.Events;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Netcool.Coding.ViewModels.Database;

public class DataTableViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{

    public string UrlPathSegment => "data-table";
    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public IObservableCollection<Column> Columns { get; set; } = new ObservableCollectionExtended<Column>();

    [Reactive]
    public DataTable ResultSet { get; set; }

    public DataTableViewModel(IScreen screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        this.WhenActivated(d =>
        {
            MessageBus.Current.Listen<TableSelectedEvent>()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(e =>
                {
                    if (e == null || e.Table == null || string.IsNullOrEmpty(e.Table.Database) || string.IsNullOrEmpty(e.Table.Name)) return;
                    var schemaReader = Locator.Current.GetService<ISchemaReader>();

                    List<Column> columns=null;
                    DataTable resultSet=null;
                    try
                    {
                        columns = schemaReader?.ReadColumns(e.Table.Database, e.Table.Schema, e.Table.Name);
                        e.Table.Columns = columns;
                        resultSet = schemaReader?.GetResultSet(e.Table, 30);
                    }
                    catch (Exception ex)
                    {
                        Growl.Error(new GrowlInfo
                        {
                            IsCustom = true,
                            Message =
                                $"Read column info failed: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}",
                            WaitTime = 5
                        });
                    }

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (columns == null || columns.Count == 0)
                            Columns.Clear();
                        else
                        {
                            Columns.Load(columns);
                            ResultSet = resultSet;
                        }
                    });

                }).DisposeWith(d);
        });


    }


}