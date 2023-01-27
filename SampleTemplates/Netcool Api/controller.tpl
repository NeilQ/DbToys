{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | string.to_pascal_case | string.to_plural) + "Controller.cs" ~}} 
{{~ classname = table.clean_name | string.to_pascal_case | string.to_singular ~}}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netcool.Api.Domain.{{ classname }};
using Netcool.Core.AspNetCore.Controllers;

namespace Netcool.Api.Controllers

[Route("{{ table.clean_name | string.to_plural | string.to_kebab_case }}")]
[Authorize]
public class {{classname | string.to_plural}}Controller :
    CrudControllerBase<{{classname}}Dto, int, {{classname}}Request, {{classname}}SaveInput>
{
    private new readonly I{{classname}}Service Service;

    public {{classname | string.to_plural}}Controller(I{{classname}}Service service) : base(service)
    {
        Service = service;
    }
    
}