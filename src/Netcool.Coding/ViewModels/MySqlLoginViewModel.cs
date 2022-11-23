using System;
using System.Reactive;
using MySql.Data.MySqlClient;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels;

public class MySqlLoginViewModel : ReactiveObject
{
    [Reactive] public string ServerIp { get; set; } = "127.0.0.1";

    [Reactive] public int Port { get; set; } = 5432;

    public Action CloseAction { get; set; }

    [Reactive] public string Username { get; set; } = "postgres";

    [Reactive] public string Password { get; set; }

    [Reactive] public bool IsConnecting { get; set; }

    public ReactiveCommand<Unit, MySqlConnectionStringBuilder> Connect { get; set; }

    public ReactiveCommand<Unit, Unit> Cancel { get; set; }

    public MySqlLoginViewModel(){}
}