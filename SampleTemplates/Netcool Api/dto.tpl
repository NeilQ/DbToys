{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | string.to_pascal_case | string.to_singular) + "Dto.cs" ~}} 
{{~ classname = table.clean_name | string.to_pascal_case | string.to_singular ~}}
{{~ func get_property_type_of_pgsql(db_type)
    case db_type
        when "int","int2","int8"
            ret "int"
        when "bytea"
            ret "byte[]"
        when "float4"
            ret "float"
        when "float8"
            ret "double"
        when "money","numeric"
            ret "decimal"
        when "bool","boolean"
            ret "bool"
        when "time","timetz","timestamp","timestamptz","date"
            ret "DateTime"
        else
            ret "string"
    end
end ~}}
{{~ func get_property_type_of_mysql(db_type)
    case db_type
        when "int","bigint","smallint"
            ret "int"
        when "image","binary","blob","mediumblob","longblob","varbinary"
            ret "byte[]"
        when "float"
            ret "float"
        when "double"
            ret "double"
        when "money","smallmoney","numeric","decimal"
            ret "decimal"
        when "bit","bool","boolean"
            ret "bool"
        when "guid"
            ret "Guid"
        when "smalldatetime","timestamp","datetime","date"
            ret "DateTime"
        else
            ret "string"
    end
end ~}}
{{~ func get_property_type_of_sql_server(db_type)
    case db_type
        when "int","bigint","smallint"
            ret "int"
        when "image","binary","timestamp","varbinary"
            ret "byte[]"
        when "real"
            ret "float"
        when "float","double"
            ret "double"
        when "money","smallmoney","numeric","decimal"
            ret "decimal"
        when "tinyint"
            ret "byte"
        when "bit"
            ret "bool"
        when "uniqueidentifier"
            ret "Guid"
        when "smalldatetime","datetime","datetime2","date","time"
            ret "DateTime"
        when "datetimeoffset"
            ret "DateTimeOffset"
        when "geography"
            ret "Microsoft.SqlServer.Types.SqlGeography"
        when "geometry"
            ret "Microsoft.SqlServer.Types.SqlGeometry"
        else
            ret "string"
    end
end ~}}
{{~ ignoredCols=["id","create_time","create_user_id","update_time","update_user_id","is_deleted","delete_time","delete_user_id"] ~}}
using System;
using System.ComponentModel.DataAnnotations;
using Netcool.Core.Services.Dto;

namespace Netcool.Api.Domain.{{ classname }}


public class {{ classname }}SaveInput : EntityDto
{ 
    {{~ for col in table.columns ~}}
    {{~ if col.description && col.description!="" ~}}
    /// <summary> 
    /// {{col.description}}
    /// </summary>
    {{~ end ~}}
    {{~ if !(ignoredCols | array.contains col.name) ~}}
    public {{ col.db_type | get_property_type_of_pgsql }} {{ col.property_name | string.to_pascal_case }} { get; set; }

    {{~ end ~}}
    {{~ end ~}}
}   

public class {{ classname }}Dto : {{ classname }}SaveInput
{

}

public class {{ classname }}Query : PageRequest
{
    
}
   
