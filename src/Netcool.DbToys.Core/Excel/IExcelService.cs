using Netcool.DbToys.Core.Database;

namespace Netcool.DbToys.Core.Excel;

public interface IExcelService
{
    public void GenerateDatabaseDictionary(IList<Table> tableList, string fileName);
}