using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Netcool.DbToys.Core;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Views.Database;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class SqlServerConnectViewModel : ObservableObject
{
    private string _server = "127.0.0.1";
    public string Server { get => _server; set => SetProperty(ref _server, value); }

    private string _username = "sa";
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    public string Password { get; set; }

    private bool _isSqlServerAuthentication = true;

    public bool IsSqlServerAuthentication
    {
        get => _isSqlServerAuthentication;
        set => SetProperty(ref _isSqlServerAuthentication, value);
    }

    public List<NameValue<bool>> AuthenticationTypes { get; set; } = new()
    {
        new NameValue<bool>("Sql Server Authentication", true),
        new NameValue<bool>("Windows Authentication", false)
    };

    private bool _isConnecting;
    public bool IsConnecting { get => _isConnecting; set => SetProperty(ref _isConnecting, value); }

    private string _errorMessage;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _hasError;
    public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }

    public IRelayCommand<SqlServerConnectDialog> ConnectCommand { get; }
    public IRelayCommand<SqlServerConnectDialog> CancelCommand { get; }

    public ISchemaReader SchemaReader { get; set; }

    public SqlServerConnectViewModel()
    {
        ConnectCommand = new RelayCommand<SqlServerConnectDialog>(dialog =>
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                TrustServerCertificate = true,
                PersistSecurityInfo = true,
                IntegratedSecurity = !IsSqlServerAuthentication,
                UserID = Username,
                Password = Password ?? ""
            };
            IsConnecting = true;
            using var sqlConn = new SqlConnection(builder.ConnectionString);
            try
            {
                sqlConn.Open();
                SchemaReader = new SqlServerSchemaReader(builder);
            }
            catch (Exception e)
            {
                HasError = true;
                ErrorMessage = e.Message;
                return;
            }
            finally
            {
                IsConnecting = false;
            }

            dialog?.Hide();
        });
        CancelCommand = new RelayCommand<SqlServerConnectDialog>(dialog =>
        {
            dialog?.Hide();
        });
    }
}