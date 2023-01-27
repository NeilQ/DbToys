{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | string.to_pascal_case | string.to_singular) + "Service.cs" ~}} 
{{~ classname = table.clean_name | string.to_pascal_case | string.to_singular ~}}
using Netcool.Core.Entities;
using Netcool.Core.Repositories;
using Netcool.Core.Services;

namespace Netcool.Api.Domain.{{classname}}

public interface I{{classname}}Service : ICrudService<{{classname}}Dto, int, {{classname}}Request>
{
    
}

public class {{ classname }}Service :
        CrudService<{{classname}}, {{classname}}Dto, int, {{classname}}Query>,
        I{{classname}}Service
{
    public {{classname}}Service(IRepository<{{classname}}> repository, IServiceAggregator serviceAggregator) :
        base(repository, serviceAggregator)
    {
        GetPermissionName = "{{ table.clean_name | string.to_kebab_case }}.view";
        UpdatePermissionName = "{{ table.clean_name | string.to_kebab_case }}.update";
        CreatePermissionName = "{{ table.clean_name | string.to_kebab_case }}.create";
        DeletePermissionName = "{{ table.clean_name | string.to_kebab_case }}.delete";
    }

    protected override IQueryable<{{classname}}> CreateFilteredQuery({{classname}}Query input)
    {
        var query = Repository.GetAll();
        //query = query.Where(t => t.id == input.id);
        return query;
    }
}