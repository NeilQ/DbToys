{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular) + "Repo.cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
using Acartons.Core.Repositories;
using Acartons.Core.Sessions;
using NPoco;

namespace Acartons.Domain.{{ classname | string.to_plural }};

public interface I{{ classname }}Repo : INPocoRepository<{{classname}}>
{ 
} 

public class {{classname}}Repo : NPocoRepository<{{classname}}>, I{{classname}}Repo
{
    public {{classname}}Repo(IUserContext userContext) : base(userContext) { }

    protected override Sql CreateQuerySql()
    {
        return base.CreateQuerySql();
    }

    protected override void ApplyQueryFilter(Sql sql, object condition) 
    {

    }
}
