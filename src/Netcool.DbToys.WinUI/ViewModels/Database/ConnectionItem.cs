using Netcool.DbToys.Core.Database;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class ConnectionItem : TreeItem
{
    private DatabaseType _databaseType;
    public DatabaseType DatabaseType { get => _databaseType; set => SetProperty(ref _databaseType, value); }

    public ConnectionItem(string name, DatabaseType databaseType) : base(name, false)
    {
        DatabaseType=databaseType;
    }

}