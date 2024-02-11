import { ValidationErrors, ValidatorFn, AbstractControl } from '@angular/forms';

export class CustomValidators {
    static patternValidator(regex: RegExp, error: ValidationErrors): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } => {
            if (!control.value) {
                // if control is empty return no error
                return null;
            }

            // test the value of the control against the regexp supplied
            const valid = regex.test(control.value);

            // if true, return no error (no error), else return error passed in the second parameter
            return valid ? null : error;
        };
    }

    static passwordMatchValidator(control: AbstractControl) {
        // get password from our password form control
        const password: string = control.get('password').value;

        // get password from our passwordConfirmation form control
        const passwordConfirmation: string = control.get('passwordConfirmation').value;

        // compare is the password math
        if (password !== passwordConfirmation) {
            // if they don't match, set an error in our passwordConfirmation form control
            control.get('passwordConfirmation').setErrors({ NoPasswordMatch: true });
        }
    }
}
