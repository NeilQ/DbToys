{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular) + "Service.cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
using Acartons.Core.Services;

namespace Acartons.Domain.{{ classname | string.to_plural }};

public interface I{{ classname }}Service : ICrudService<{{classname}}, {{classname}}Dto, {{classname}}Save>
{ 
} 

public class {{classname}}Service : UowCrudServiceBase<{{classname}}, {{classname}}Dto, {{classname}}Save>, I{{classname}}Service
{
    public {{classname}}Service(I{{classname}}Repo repository, IUowServiceDependencies dependencies): 
            base(repository, dependencies)
    {
    }
}