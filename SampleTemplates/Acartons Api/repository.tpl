{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular) + "Repo.cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
using System.Linq;
using Acartons.Core.EfCore;
using Acartons.Domain.EfCore;

namespace Acartons.Domain.{{ classname | string.to_plural }};

public interface I{{ classname }}Repo : IEfCoreRepository<{{classname}}>
{ 
} 

public class {{classname}}Repo : EfCoreRepositoryBase<{{classname}}>, I{{classname}}Repo
{
    public {{classname}}Repo(AcartonsDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<{{classname}}> CreateFilteredQuery(object condition)
    {
        var query = base.CreateFilteredQuery(condition);
        if (condition is not {{classname}}Query req) return query;
        //query = query.Where(CreateCompanyIdFilter(req.CompanyId));
        return query;
    }
}
