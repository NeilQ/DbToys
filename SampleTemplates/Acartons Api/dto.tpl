{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular) + "Dto.cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
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
{{~ 
ignoredCols=["id","add_user","add_time","update_time","update_user","marked_for_delete","delete_time","delete_user"]
~}}

namespace Acartons.Domain.{{ classname | string.to_plural }};

public class {{ classname }}Save
{ 
    {{~ for col in table.columns ~}}   
      {{~ if !(ignoredCols | array.contains col.name) ~}}
        {{~ if col.description && col.description!="" ~}}
    /// <summary> 
    /// {{col.description}}
    /// </summary>
        {{~ end ~}}
    public {{ col.db_type | get_property_type_of_pgsql }} {{ col.property_name | string.to_pascal_case }} { get; set; }

      {{~ end ~}}
    {{~ end ~}}
}   

public class {{ classname }}Dto : {{ classname }}Save
{
    public int Id { get; set; }
}

public class {{ classname }}Query
{
}