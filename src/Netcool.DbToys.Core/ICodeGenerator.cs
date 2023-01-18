using Netcool.DbToys.Core.Database;

namespace Netcool.DbToys.Core;

public interface ICodeGenerator
{
    public string GenerateFromTable(Table table, string templateText);

}

public class CodeGenerator : ICodeGenerator
{
    public string GenerateFromTable(Table table, string templateText)
    {
        throw new NotImplementedException();
    }
}