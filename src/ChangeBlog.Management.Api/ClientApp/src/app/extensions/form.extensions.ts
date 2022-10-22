import {UntypedFormGroup} from '@angular/forms';
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
UntypedFormGroup.prototype.resetValidation = function (this: UntypedFormGroup) {
  for (const key of Object.keys(this.controls)) {
    const control = this.controls[key];
    control.setErrors(null,);
    control.markAsUntouched();
  }
}

UntypedFormGroup.prototype.setServerError = function (this: UntypedFormGroup, errorMessages: ErrorMessage[]) {
  for (const e of errorMessages.filter((x: ErrorMessage) => !!x.property)) {
    const formControl = this.get(e.property!);
    const validationMessage = new ValidationError({message: e.message!});

    formControl?.setErrors(validationMessage);
  }
}

UntypedFormGroup.prototype.showErrorMessage = function (this: UntypedFormGroup, formControlName: string) {
  const control = this.get(formControlName);
  return (control?.invalid ?? false) && control?.errors instanceof ValidationError;
}

UntypedFormGroup.prototype.getServerErrorMessage = function (this: UntypedFormGroup, formControlName: string) {
  return this.get(formControlName)?.errors?.serverError?.message;
}
