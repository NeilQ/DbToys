{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_kebab_case | string.to_singular) + ".component.html" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ 
# How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction 
# Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates
# Press 'F1' to show editor commands
~}}
{{~ 
ignoredCols=["id","add_user","add_time","update_time","update_user","marked_for_delete","delete_time","delete_user"]
~}}
<div nz-row>
  <div nz-col nzSpan="24">
    <nz-card class="viewport100-min" [nzBordered]="false">
      <form nz-form [nzLayout]="'inline'">
      <!-- query controls -->
      </form>

      <ul class="btn-list clearfix">
        <li>
          <button type="button" class="btn btn-success" [authCode]="AuthCode.{{classname}}Add" (click)="onAdd()">
            <i class="fa fa-plus"></i>
          </button>
        </li>
        <li>
          <button type="button" class="btn btn-info" [authCode]="AuthCode.{{classname}}Edit" (click)="onUpdate()">
            <i class="fa fa-pencil-square-o"></i>
          </button>
        </li>
        <li>
          <button type="button" (click)="queryData()" class="btn btn-primary">
            <i class="fa fa-search"></i>
          </button>
        </li>
        <li class="float-end">
          <button type="button" class="btn btn-danger" [authCode]="AuthCode.{{classname}}Delete" (click)="onDelete()">
            <i class="fa fa-trash-o"></i>
          </button>
        </li>
       </ul>

      <nz-table #ajaxTable nzBordered nzShowSizeChanger [nzFrontPagination]="false" [nzData]="datasource"
              [nzLoading]="loading" [nzTotal]="total" [(nzPageIndex)]="currPage" [(nzPageSize)]="currSize"
              (nzPageIndexChange)="loadLazy()" (nzPageSizeChange)="loadLazy()" nzSize="middle">
        <thead>
        <tr>
          <th nzShowCheckbox [(nzChecked)]="allChecked" [nzIndeterminate]="indeterminate"
            (nzCheckedChange)="checkAll($event)"></th>
            {{~ for col in table.columns ~}}  
            {{~ if !(ignoredCols | array.contains col.name) && !(col.name | string.ends_with "id") ~}}
              {{~ if col.description && col.description!="" ~}}
          <th>{{ col.description }}</th>
              {{~ else ~}}
          <th>{{ col.name }}</th>	
              {{~ end ~}}
            {{~ end ~}}
            {{~ end ~}}
        </tr>
        </thead>
        <tbody>
        <tr *ngFor="let data of ajaxTable.data">
          <td nzShowCheckbox [(nzChecked)]="data.checked" [nzDisabled]="data.disabled"
              (nzCheckedChange)="refreshStatus()"></td>
            {{~ for col in table.columns ~}}   
            {{~ if !(ignoredCols | array.contains col.name) && !(col.name | string.ends_with "id") ~}}
          <td>{%{{{}%}data.{{col.property_name | string.to_camel_case}}{%{}}}%}</td>
            {{~ end ~}}
            {{~ end ~}}
        </tr>
        </tbody>
      </nz-table>
    </nz-card>
  </div>
</div>