using System;
using System.Data.Common;
using System.Reactive;
using HandyControl.Controls;
using HandyControl.Data;
using Netcool.Workshop.Database;
using Netcool.Workshop.Views;
using ReactiveUI;
using Splat;

namespace Netcool.Workshop.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router
        {
            get;
        }
        public MainWindowViewModel()
        {
            Router = new RoutingState();

            RegisterDependencies();

            Router.Navigate.Execute(new LayoutViewModel(this));
        }

        public void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new LayoutView(), typeof(IViewFor<LayoutViewModel>));
            Locator.CurrentMutable.Register(() => new DatabasePanelView(), typeof(IViewFor<DatabasePanelViewModel>));
            Locator.CurrentMutable.Register(() => new ConnectionItemView(), typeof(IViewFor<ConnectionItem>));
            Locator.CurrentMutable.Register(() => new DatabaseItemView(), typeof(IViewFor<DatabaseItem>));
            Locator.CurrentMutable.Register(() => new TableItemView(), typeof(IViewFor<TableItem>));
            DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);

            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                Growl.Fatal(new GrowlInfo
                {
                    Message = "Fatal: " + ex.Message,
                    IsCustom = true,
                    StaysOpen = false,
                    WaitTime = 5
                });

                this.Log().Error(ex);
            });
        }

    }
}
