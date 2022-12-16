using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Netcool.Coding.Core.Database;
using Netcool.Coding.WinUI.Views.Database;
using Npgsql;

namespace Netcool.Coding.WinUI.ViewModels.Database;

public class PostgreSqlConnectViewModel : ObservableRecipient
{
    private string _serverIp = "127.0.0.1";
    public string ServerIp { get => _serverIp; set => SetProperty(ref _serverIp, value); }

    private int _port = 5432;
    public int Port { get => _port; set => SetProperty(ref _port, value); }

    private string _username = "postgres";
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    public string Password { get; set; }

    private bool _isConnecting;
    public bool IsConnecting { get => _isConnecting; set => SetProperty(ref _isConnecting, value); }

    private string _errorMessage;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _hasError;
    public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }

    public IRelayCommand<PostgreSqlConnectDialog> ConnectCommand { get; }
    public IRelayCommand<PostgreSqlConnectDialog> CancelCommand { get; }

    public ISchemaReader SchemaReader { get; set; }

    public PostgreSqlConnectViewModel()
    {
        ConnectCommand = new RelayCommand<PostgreSqlConnectDialog>(dialog =>
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
            try
            {
                sqlConn.Open();
                SchemaReader = new PostgreSqlSchemaReader(builder);
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
        CancelCommand = new RelayCommand<PostgreSqlConnectDialog>(dialog =>
        {
            dialog?.Hide();
        });
    }
}