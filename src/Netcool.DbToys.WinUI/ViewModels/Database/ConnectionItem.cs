using Netcool.DbToys.Core.Database;

namespace Netcool.DbToys.WinUI.ViewModels.Database;

public class ConnectionItem : TreeItem
{
    private DataBaseType _dataBaseType;
    public DataBaseType DataBaseType { get => _dataBaseType; set => SetProperty(ref _dataBaseType, value); }

    public ConnectionItem(string name, DataBaseType databaseType) : base(name, false)
    {
        DataBaseType=databaseType;
    }

}