using System;
using System.Data.Common;
using System.Reactive;
using HandyControl.Controls;
using HandyControl.Data;
using Netcool.Workshop.Database;
using ReactiveUI;
using Splat;

namespace Netcool.Workshop.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
     
        public MainWindowViewModel()
        {
            RegisterDependencies();
        }

        public void RegisterDependencies()
        {
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
