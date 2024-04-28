{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_plural) + "Controller.cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Acartons.Api.Controllers.Base;
using Acartons.Domain.{{ classname | string.to_plural }};

namespace Acartons.Api.Controllers;

[Route("api/{{ classname | string.to_plural | string.to_snake_case }}")]
[ApiExplorerSettings(GroupName = "")]
[Authorize]
public class {{classname | string.to_plural}}Controller :
    CrudControllerBase<{{classname}}, {{classname}}Dto, {{classname}}Query, {{classname}}Save>
{
    public {{classname | string.to_plural}}Controller(I{{classname}}Service service, IControllerDependencies dependencies)
        : base(service, dependencies)
    {
    }

}