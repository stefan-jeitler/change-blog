import {UntypedFormGroup} from '@angular/forms';
import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import {ValidationError} from "../models/validation-error";
import ErrorMessage = ChangeBlogManagementApi.ErrorMessages;

declare module "@angular/forms" {

    interface FormGroup {
        resetValidation(this: FormGroup): void;

        setErrors(this: FormGroup, errorMessages: ErrorMessage[]): void;

        showErrorMessage(this: FormGroup, formControlName: string): boolean;

        getErrorMessages(this: FormGroup, formControlName: string): string;
    }
}

UntypedFormGroup.prototype.resetValidation = function (this: UntypedFormGroup) {
    for (const key of Object.keys(this.controls)) {
        const control = this.controls[key];
        control.setErrors(null);
        control.markAsUntouched();
    }
}

UntypedFormGroup.prototype.setErrors = function (this: UntypedFormGroup, errorMessages: ErrorMessage[]) {
    for (const e of errorMessages.filter((x: ErrorMessage) => !!x.property)) {
        const formControl = this.get(e.property!);
        const fieldValidationErrors = new ValidationError(e?.messages ?? []);

        formControl?.setErrors(fieldValidationErrors);
    }
}

UntypedFormGroup.prototype.showErrorMessage = function (this: UntypedFormGroup, formControlName: string) {
    const control = this.get(formControlName);
    return (control?.invalid ?? false) && control?.errors instanceof ValidationError;
}

UntypedFormGroup.prototype.getErrorMessages = function (this: UntypedFormGroup, formControlName: string) {
    return this.get(formControlName)?.errors?.messages ?? [];
}
