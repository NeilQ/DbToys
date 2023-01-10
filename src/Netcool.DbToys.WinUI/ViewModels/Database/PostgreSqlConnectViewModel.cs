using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Services;
using Netcool.DbToys.WinUI.Views.Database;
using Npgsql;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class PostgreSqlConnectViewModel : ObservableRecipient
{
    private string _server = "127.0.0.1";
    public string Server { get => _server; set => SetProperty(ref _server, value); }

    private int _port = 5432;
    public int Port { get => _port; set => SetProperty(ref _port, value); }

    private string _username = "postgres";
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    public string Password { get; set; }

    private bool _savePassword;
    public bool SavePassword { get => _savePassword; set => SetProperty(ref _savePassword, value); }

    private bool _isConnecting;
    public bool IsConnecting { get => _isConnecting; set => SetProperty(ref _isConnecting, value); }

    private string _errorMessage;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _hasError;
    public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }

    public IRelayCommand<PostgreSqlConnectDialog> ConnectCommand { get; }
    public IRelayCommand<PostgreSqlConnectDialog> CancelCommand { get; }

    public ObservableCollection<DatabaseAccount> Accounts { get; set; } = new();

    private int _selectedAccountIndex;
    public int SelectedAccountIndex
    {
        get => _selectedAccountIndex;
        set
        {
            SetProperty(ref _selectedAccountIndex, value);
            if (value >= 0)
            {
                var account = Accounts[value];
                if (account != null)
                {
                    Server = account.Server;
                    Port = account.Port;
                    Username = account.Username;
                    string password = null;
                    if (!string.IsNullOrEmpty(account.Password))
                    {
                        SavePassword = true;
                        password = _accountHistory.DecryptPassword(account.Password);
                    }
                    else
                    {
                        SavePassword = false;
                    }
                    Password = password;
                    PasswordChanged?.Invoke(password);
                }

            }
        }
    }

    private readonly IDatabaseAccountHistory _accountHistory;

    public ISchemaReader SchemaReader { get; set; }

    public Action<string> PasswordChanged { get; set; }

    public PostgreSqlConnectViewModel(IDatabaseAccountHistory accountHistory)
    {
        _accountHistory = accountHistory;
        ConnectCommand = new RelayCommand<PostgreSqlConnectDialog>(ConnectDatabase);
        CancelCommand = new RelayCommand<PostgreSqlConnectDialog>(dialog =>
        {
            dialog?.Hide();
        });
    }

    protected override async void OnActivated()
    {
        HasError = false;
        SchemaReader = null;
        var accounts = await _accountHistory.GetAllAsync(DatabaseType.PostgreSql);
        Accounts.Clear();
        if (accounts?.Count > 0)
        {
            accounts.ForEach(Accounts.Add);
            SelectedAccountIndex = 0;
        }
    }

    private void ConnectDatabase(PostgreSqlConnectDialog dialog)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Server,
            Port = Port,
            Timeout = 3,
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
            sqlConn.Close();
            _accountHistory.Add(new DatabaseAccount
            {
                DatabaseType = DatabaseType.PostgreSql,
                Server = Server,
                Port = Port,
                Username = Username,
                Password = Password
            }, SavePassword);
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
    }
}