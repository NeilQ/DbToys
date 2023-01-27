using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Netcool.DbToys.Core;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.Services;
using Netcool.DbToys.Views.Dialogs;

namespace Netcool.DbToys.ViewModels.Dialogs;

public class SqlServerConnectViewModel : ObservableRecipient
{
    private string _server = "127.0.0.1";
    public string Server { get => _server; set => SetProperty(ref _server, value); }

    private string _username = "sa";
    public string Username { get => _username; set => SetProperty(ref _username, value); }

    public string Password { get; set; }

    private bool _savePassword;
    public bool SavePassword { get => _savePassword; set => SetProperty(ref _savePassword, value); }

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

    public Action<string> PasswordChanged { get; set; }

    private readonly IDatabaseAccountHistory _accountHistory;

    public ISchemaReader SchemaReader { get; set; }

    public SqlServerConnectViewModel(IDatabaseAccountHistory accountHistory)
    {
        _accountHistory = accountHistory;
        ConnectCommand = new RelayCommand<SqlServerConnectDialog>(ConnectDatabase);
        CancelCommand = new RelayCommand<SqlServerConnectDialog>(dialog =>
        {
            dialog?.Hide();
        });
    }

    private void ConnectDatabase(SqlServerConnectDialog dialog)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Server,
            ConnectTimeout = 3,
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
            sqlConn.Close();
            _accountHistory.Add(
                new DatabaseAccount
                {
                    DatabaseType = DatabaseType.SqlServer,
                    Server = Server,
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

    protected override async void OnActivated()
    {
        HasError = false;
        SchemaReader = null;
        var accounts = await _accountHistory.GetAllAsync(DatabaseType.SqlServer);
        Accounts.Clear();
        if (accounts?.Count > 0)
        {
            accounts.ForEach(Accounts.Add);
            SelectedAccountIndex = 0;
        }
    }
}