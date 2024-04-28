{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_kebab_case | string.to_singular) + ".component.ts" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
import { Component, ViewEncapsulation } from '@angular/core';
import { CrudTableComponentBase, {{classname}}, IdNamePair } from '@models';
import { AuthService, {{classname}}Service, NotificationsService } from '@services';
import { NzModalService } from "ng-zorro-antd/modal";
import { {{classname}}EditComponent } from "./edit/{{classname | string.to_kebab_case}}-edit.component";

@Component({
  selector: '{{classname | string.to_kebab_case}}',
  templateUrl: '{{classname | string.to_kebab_case}}.component.html',
  encapsulation: ViewEncapsulation.None
})
export class {{classname}}Component extends CrudTableComponentBase<{{classname}}> {

  query = {
    companyId: null,
  };

  constructor(
    protected apiService: {{classname}}Service,
    protected notifyService: NotificationsService,
    protected modalService: NzModalService,
    protected authService: AuthService) {
    super(authService, notifyService, apiService, modalService);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.editTitle = "";
    //this.editModalWidth = 600;
    //this.deleteMessage = "";
    this.editComponent = {{classname}}EditComponent;
    this.editComponentParams = {companyId: this.companyId};
  }

  onCompanyChanged(company: IdNamePair) {
    super.onCompanyChanged(company);
  }

}
