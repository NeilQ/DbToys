using Netcool.Coding.Core.Database;

namespace Netcool.Coding.WinUI.ViewModels.Database;

public class ConnectionItem : TreeItem
{
    private DataBaseType _dataBaseType;
    public DataBaseType DataBaseType { get => _dataBaseType; set => SetProperty(ref _dataBaseType, value); }

    public ConnectionItem(string name, DataBaseType databaseType) : base(name, false)
    {
        DataBaseType=databaseType;
    }

}