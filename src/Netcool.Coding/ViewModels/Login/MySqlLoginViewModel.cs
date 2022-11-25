using System;
using System.Reactive;
using HandyControl.Controls;
using HandyControl.Data;
using MySql.Data.MySqlClient;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels;

public class MySqlLoginViewModel : ReactiveObject
{
    [Reactive] public string ServerIp { get; set; } = "127.0.0.1";

    [Reactive] public int Port { get; set; } = 3306;

    public Action CloseAction { get; set; }

    [Reactive] public string Username { get; set; } = "root";

    [Reactive] public string Password { get; set; }

    [Reactive] public bool IsConnecting { get; set; }

    public ReactiveCommand<Unit, MySqlConnectionStringBuilder> Connect { get; set; }

    public ReactiveCommand<Unit, Unit> Cancel { get; set; }

    public MySqlLoginViewModel()
    {
        Cancel = ReactiveCommand.Create(() =>
        {
            CloseAction?.Invoke();
        });

      
        Connect = ReactiveCommand.Create(() =>
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = ServerIp,
                Port = (uint)Port,
                PersistSecurityInfo = true,
                UserID = Username,
                Password = Password,
            };
            IsConnecting = true;
            using var sqlConn = new MySqlConnection(builder.ConnectionString);
            sqlConn.Open();
            IsConnecting = false;
            Growl.Success("MySql Connected");
            CloseAction?.Invoke();
            return builder;
        });

        Connect.ThrownExceptions.Subscribe(ex =>
        {
            IsConnecting = false;
            Growl.Error(new GrowlInfo
            {
                IsCustom = true,
                Message = $"Connect failed: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}",
                WaitTime = 5
            });
        });
    }
}