using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Views.Database;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class MysqlConnectViewModel : ObservableObject
{
    private string _serverIp = "127.0.0.1";
    public string ServerIp { get => _serverIp; set => SetProperty(ref _serverIp, value); }

    private int _port = 3306;
    public int Port { get => _port; set => SetProperty(ref _port, value); }

    private string _username = "root";
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    public string Password { get; set; }

    private bool _isConnecting;
    public bool IsConnecting { get => _isConnecting; set => SetProperty(ref _isConnecting, value); }

    private string _errorMessage;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _hasError;
    public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }

    public IRelayCommand<MysqlConnectDialog> ConnectCommand { get; }
    public IRelayCommand<MysqlConnectDialog> CancelCommand { get; }

    public ISchemaReader SchemaReader { get; set; }

    public MysqlConnectViewModel()
    {
        ConnectCommand = new RelayCommand<MysqlConnectDialog>(dialog =>
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
            try
            {
                sqlConn.Open();
                SchemaReader = new MySqlSchemaReader(builder);
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
        CancelCommand = new RelayCommand<MysqlConnectDialog>(dialog =>
        {
            dialog?.Hide();
        });
    }
}