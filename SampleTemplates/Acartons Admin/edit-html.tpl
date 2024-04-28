{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_kebab_case | string.to_singular) + "-edit.html" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ 
ignoredCols=["id","add_user","add_time","update_time","update_user","marked_for_delete","delete_time","delete_user"]
~}}
<form #editForm="ngForm" nz-form>
  {{~ for col in table.columns ~}}  
  {{~ if !(ignoredCols | array.contains col.name) && !(col.name | string.ends_with "id") ~}}
  <nz-form-item>
    <nz-form-label [nzSpan]="5">{{ col.description }}</nz-form-label>
    <nz-form-control nzHasFeedback [nzSpan]="18">
    {{~ if (col.db_type | get_js_property_type_of_pgsql)=="string" ~}}
      <input nz-input [(ngModel)]="entity.{{col.property_name | string.to_camel_case}}" name="{{col.property_name | string.to_camel_case}}"/>
    {{~ end ~}}
    {{~ if (col.db_type | get_js_property_type_of_pgsql)=="number" ~}}
      <nz-input-number [nzMin]="0" [(ngModel)]="entity.{{col.property_name | string.to_camel_case}}" name="{{col.property_name | string.to_camel_case}}" required></nz-input-number>
    {{~ end ~}}
    </nz-form-control>
  </nz-form-item>
  
  {{~ end ~}}
  {{~ end ~}}

  <nz-form-item>
    <nz-form-control [nzSpan]="18" nzOffset="5">
      <button nz-button nzType="primary" [nzLoading]="submitted" [disabled]="!editForm.valid" (click)="save()">保存</button>
    </nz-form-control>
  </nz-form-item>
</form>