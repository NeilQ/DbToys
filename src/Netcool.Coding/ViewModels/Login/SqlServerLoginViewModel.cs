using System;
using System.Collections.Generic;
using System.Reactive;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Data.SqlClient;
using Netcool.DbToys.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Netcool.Coding.ViewModels;

public class SqlServerLoginViewModel : ReactiveObject
{
    public Action CloseAction { get; set; }

    [Reactive] public string Server { get; set; } = "127.0.0.1";

    [Reactive] public string Username { get; set; } = "sa";

    public string Password { get; set; }

    [Reactive] public bool IsConnecting { get; set; }

    [Reactive] public bool IsSqlServerAuthentication { get; set; } = true;

    [Reactive]
    public List<NameValue<bool>> AuthenticationTypes { get; set; } = new()
    {
        new NameValue<bool>("Sql Server Authentication", true),
        new NameValue<bool>("Windows Authentication", false)
    };

    public ReactiveCommand<Unit, SqlConnectionStringBuilder> Connect { get; set; }

    public ReactiveCommand<Unit, Unit> Cancel { get; set; }

    public SqlServerLoginViewModel()
    {
        Cancel = ReactiveCommand.Create(() =>
        {
            CloseAction?.Invoke();
        });
        this.WhenAnyValue(t => t.IsSqlServerAuthentication).Subscribe(type =>
        {
        });

        Connect = ReactiveCommand.Create(() =>
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                TrustServerCertificate = true,
                PersistSecurityInfo = true,
                IntegratedSecurity = !IsSqlServerAuthentication,
                Password = Password,
                UserID = Username
            };
            IsConnecting = true;
            using var sqlConn = new SqlConnection(builder.ConnectionString);
            sqlConn.Open();
            IsConnecting = false;
            Growl.Success("SqlServer Connected");
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