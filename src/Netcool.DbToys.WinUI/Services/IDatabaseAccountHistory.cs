using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Netcool.DbToys.Core.Database;
using Netcool.DbToys.WinUI.Models;

namespace Netcool.DbToys.WinUI.Services;

public class DatabaseAccount
{
    public DatabaseType DatabaseType { get; set; }

    public string Server { get; set; }

    public int Port { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}

public interface IDatabaseAccountHistory
{
    void Add(DatabaseAccount account, bool savePassword = false);

    Task<List<DatabaseAccount>> GetAllAsync(DatabaseType? databaseType);
}

public class DatabaseAccountHistory : SettingsServiceBase, IDatabaseAccountHistory
{
    public override string SettingFileName { get; set; } = "database.json";

    public const string SecretKey = "Secret";
    public const string DatabaseAccountKey = "DatabaseAccounts";

    private const int MaxCapacity = 10;

    public DatabaseAccountHistory(IFileService fileService, IOptions<SettingsOptions> options) : base(fileService,
        options)
    {
    }

    public async void Add(DatabaseAccount account, bool savePassword = false)
    {
        var accounts = await ReadSettingAsync<List<DatabaseAccount>>(DatabaseAccountKey) ?? new List<DatabaseAccount>();
        if (savePassword)
            account.Password = await EncryptPassword(account.Password);
        else
            account.Password = null;

        var cache = accounts.FirstOrDefault(t => t.DatabaseType == account.DatabaseType
                                                 && t.Username == account.Username
                                                 && t.Server == account.Server
                                                 && t.Port == account.Port);
        if (cache != null) accounts.Remove(cache);
        accounts.Insert(0,account);

        if (accounts.Count > MaxCapacity)
        {
            accounts.RemoveRange(MaxCapacity - 1, MaxCapacity - accounts.Count);
        }

        await SaveSettingAsync(DatabaseAccountKey, accounts);
    }

    public async Task<List<DatabaseAccount>> GetAllAsync(DatabaseType? databaseType)
    {
        var list = await ReadSettingAsync<List<DatabaseAccount>>(DatabaseAccountKey);
        if (databaseType != null)
        {
            list = list?.Where(t => t.DatabaseType == databaseType.Value).ToList();
        }

        return list;
    }


    private async Task<string> EncryptPassword(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        var secret = await ReadSettingAsync<string>(SecretKey);
        if (string.IsNullOrEmpty(secret))
        {
            secret = RandomString(16);
            await SaveSettingAsync(SecretKey, secret);
        }
        var entropy = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(str);
        var protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
        return protectedData;
    }

    private async Task<string> DecryptPassword(string str)
    {
        var secret = await ReadSettingAsync<string>(SecretKey);
        var protectedData = Convert.FromBase64String(str);
        var entropy = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
        return data;
    }

    private string RandomString(int length)
    {
        var chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[4 * length];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        var result = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;
            result.Append(chars[idx]);
        }

        return result.ToString();
    }


}