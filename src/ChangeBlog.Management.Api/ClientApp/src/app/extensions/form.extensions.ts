import {FormGroup} from '@angular/forms';
declare module "@angular/forms" {
  interface FormGroup {
    resetValidation(this:FormGroup) : void;
  }
}
FormGroup.prototype.resetValidation = function(this:FormGroup) {
  for(const key of Object.keys(this.controls)) {
    const control = this.controls[key];
    control.setErrors(null);
  }
}
