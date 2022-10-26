using System;
using System.Reactive;
using HandyControl.Controls;
using HandyControl.Data;
using Npgsql;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels
{
    public class PostgreSqlLoginViewModel : ReactiveObject
    {
        [Reactive] public string ServerIp { get; set; } = "127.0.0.1";

        [Reactive] public int Port { get; set; } = 5432;

        public Action CloseAction { get; set; }

        [Reactive] public string Username { get; set; } = "postgres";

        [Reactive] public string Password { get; set; }

        [Reactive] public bool IsConnecting { get; set; }

        public ReactiveCommand<Unit, NpgsqlConnectionStringBuilder> Connect { get; set; }

        public ReactiveCommand<Unit, Unit> Cancel { get; set; }

        public PostgreSqlLoginViewModel()
        {
            Cancel = ReactiveCommand.Create(() =>
            {
                CloseAction?.Invoke();
            });

            /*
            var canConnect = this.WhenAnyValue(t => t.ServerIp, t => t.Port, t => t.Username, t => t.IsConnecting,
                (ip, port, username, isConnecting) =>
                    !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(username) && port > 0 && !isConnecting);
            */

            Connect = ReactiveCommand.Create(() =>
            {
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = ServerIp,
                    Port = Port,
                    PersistSecurityInfo = true,
                    Username = Username,
                    Password = Password,
                    ClientEncoding = "utf8"
                };
                IsConnecting = true;
                using var sqlConn = new NpgsqlConnection(builder.ConnectionString);
                sqlConn.Open();
                IsConnecting = false;
                Growl.Success("PostgreSql Connected");
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
}