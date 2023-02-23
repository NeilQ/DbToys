{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_kebab_case | string.to_singular) + ".type.ts" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
{{~ 
ignoredCols=["add_user","add_time","update_time","update_user","marked_for_delete","delete_time","delete_user"]
~}}

export class {{classname}} {
  {{~ for col in table.columns ~}}   
  {{~ if !(ignoredCols | array.contains col.name) ~}}
  {{ col.property_name | string.to_camel_case }}: {{ col.db_type | get_js_property_type_of_pgsql }};
  {{~ end ~}}
  {{~ end ~}}
}