using DbToys.Core.Database;

namespace DbToys.Core.Excel;

public interface IExcelService
{
    public void GenerateDatabaseDictionary(IList<Table> tableList, string fileName);
}