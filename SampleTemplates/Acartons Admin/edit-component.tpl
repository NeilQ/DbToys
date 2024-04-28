{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_kebab_case | string.to_singular) + "-edit.component.ts" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
import {
  Component, Output, EventEmitter, ViewChild, Input, ViewEncapsulation, OnInit
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { NzModalRef } from "ng-zorro-antd/modal";
import { NotificationsService, {{classname}}Service } from '@services';
import { {{classname}}, ViewAction, ModalComponentBase } from '@models';

@Component({
  selector: '{{classname | string.to_kebab_case}}-edit',
  templateUrl: '{{classname | string.to_kebab_case}}-edit.html',
  encapsulation: ViewEncapsulation.None
})
export class {{classname}}EditComponent extends ModalComponentBase implements OnInit {

  action: ViewAction;
  ViewAction = ViewAction;

  @Output() onSuccess: EventEmitter<any> = new EventEmitter();
  @Input() companyId: number;
  @ViewChild('editForm') editForm: NgForm;

  submitted: boolean = false;

  entity: {{classname}} = new {{classname}}();
  currentId: number;

  constructor(
    private modal: NzModalRef,
    private apiService: {{classname}}Service,
    protected notifyService: NotificationsService) {
    super();
  }

  changeViewAction(selectedData: any, action: ViewAction) {
    this.submitted = false;
    this.action = action;

    switch (this.action) {
      default:
      case ViewAction.Add:
        this.entity.companyId = this.companyId;
        break;
      case ViewAction.Update:
        this.currentId = selectedData.id;

        this.apiService.get(this.currentId)
          .then(data => {
            this.entity = data;
          })
          .catch(err => {
            this.notifyService.error('Error', err);
          });
        break;
    }
  }

  save() {
    if (!this.editForm.valid) {
      return;
    }
    this.submitted = true;

    switch (this.action) {
      default:
      case ViewAction.Add:
        this.apiService.add(this.entity)
          .then(() => {
            this.onSuccess.emit(null);
            this.modal.destroy(true);
          })
          .catch((err) => {
            this.notifyService.error('Error', err);
            this.submitted = false;
          });
        break;
      case ViewAction.Update:
        this.apiService.update(this.entity.id, this.entity)
          .then(() => {
            this.onSuccess.emit(null);
            this.modal.destroy(true);
          })
          .catch((err) => {
            this.notifyService.error('Error', err);
            this.submitted = false;
          });
        break;
    }
  }

}
