import {FormGroup} from '@angular/forms';
import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import {ValidationError} from "../models/validation-error";
import ErrorMessage = ChangeBlogManagementApi.ErrorMessage;

declare module "@angular/forms" {

  interface FormGroup {
    resetValidation(this: FormGroup): void;

    setServerError(this: FormGroup, errorMessages: ErrorMessage[]): void;

    showErrorMessage(this: FormGroup, formControlName: string): boolean;

    getServerErrorMessage(this: FormGroup, formControlName: string): string;
  }
}
FormGroup.prototype.resetValidation = function (this: FormGroup) {
  for (const key of Object.keys(this.controls)) {
    const control = this.controls[key];
    control.setErrors(null, {
      emitEvent: true
    });
    control.reset();
    control.markAsUntouched();
  }
}

FormGroup.prototype.setServerError = function (this: FormGroup, errorMessages: ErrorMessage[]) {
  for (const e of errorMessages.filter((x: ErrorMessage) => !!x.property)) {
    const formControl = this.get(e.property!);
    const validationMessage = new ValidationError({message: e.message!});

    formControl?.setErrors(validationMessage);
  }
}

FormGroup.prototype.showErrorMessage = function (this: FormGroup, formControlName: string) {
  return this.get(formControlName)?.invalid ?? false;
}

FormGroup.prototype.getServerErrorMessage = function (this: FormGroup, formControlName: string) {
  return this.get(formControlName)?.errors?.serverError?.message;
}
