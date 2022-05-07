import {FormGroup} from '@angular/forms';
import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import ErrorMessage = ChangeBlogManagementApi.ErrorMessage;
import {ValidationError} from "../types/validation-error";

declare module "@angular/forms" {

  interface FormGroup {
    resetValidation(this:FormGroup) : void;
    setServerError(this:FormGroup, errorMessages: ErrorMessage[]): void;
  }
}
FormGroup.prototype.resetValidation = function(this:FormGroup) {
  for(const key of Object.keys(this.controls)) {
    const control = this.controls[key];
    control.setErrors(null);
  }
}

FormGroup.prototype.setServerError = function (this:FormGroup, errorMessages: ErrorMessage[]) {
  for (const e of errorMessages.filter((x: ErrorMessage) => !!x.property)) {
    const formControl = this.get(e.property!.toLowerCase());
    const validationMessage = new ValidationError({message: e.message!});

    formControl?.setErrors(validationMessage);
  }
}
