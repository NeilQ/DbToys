using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using DynamicData.Binding;
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
    public DataTableViewModel(IScreen screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        this.WhenActivated(d =>
        {
            MessageBus.Current.Listen<TableSelectedEvent>()
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(e =>
                {
                    if (e == null || string.IsNullOrEmpty(e.Database) || string.IsNullOrEmpty(e.TableName)) return;
                    var schemaReader = Locator.Current.GetService<ISchemaReader>();
                    var columns = schemaReader?.ReadColumns(e.Database, e.TableName);
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (columns == null || columns.Count == 0)
                            Columns.Clear();
                        else
                            Columns.Load(columns);

                    });
                }).DisposeWith(d);
        });


    }


}