import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { lastValueFrom } from 'rxjs';

import { IFormCustomClass } from '@data/schema/generic/form';

import { AuthService } from '@data/service/auth/auth.service';

import { CustomValidators } from '@shared/util/custom-validators';

@Component({
    selector: 'app-change-password-modal',
    templateUrl: './change-password-modal.component.html',
    styleUrls: ['./change-password-modal.component.scss'],
})
export class ChangePasswordModalComponent implements OnInit {
    @Output() onSubmit = new EventEmitter();
    changePasswordForm: FormGroup;
    isFormProcessing = false;

    textInputCustomClasses: IFormCustomClass = {
        formGroup: 'form-group row m-0 mb-2',
        label: 'col-sm-5 col-form-label px-0 fs-14',
        input: 'form-control input-h-3 fs-14',
    };

    constructor(private formBuilder: FormBuilder, public bsModalRef: BsModalRef, private authService: AuthService) {}

    ngOnInit() {
        this.initializeForm();
    }

    onHideClick() {
        this.bsModalRef.hide();
    }

    private initializeForm(): void {
        this.changePasswordForm = this.formBuilder.group(
            {
                currentPassword: ['', Validators.required],
                password: [
                    '',
                    Validators.compose([
                        Validators.required,
                        // check whether the entered password has a number
                        CustomValidators.patternValidator(/\d/, {
                            hasNumber: true,
                        }),
                        // check whether the entered password has upper case letter
                        CustomValidators.patternValidator(/[A-Z]/, {
                            hasCapitalCase: true,
                        }),
                        // check whether the entered password has a lower case letter
                        CustomValidators.patternValidator(/[a-z]/, {
                            hasSmallCase: true,
                        }),
                        // check whether the entered password has a special character
                        CustomValidators.patternValidator(/[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/, {
                            hasSpecialCharacters: true,
                        }),
                        Validators.minLength(8),
                    ]),
                ],
                passwordConfirmation: ['', Validators.compose([Validators.required])],
            },
            {
                // check whether our password and confirm password match
                validator: CustomValidators.passwordMatchValidator,
            },
        );
    }

    get f() {
        return this.changePasswordForm.controls;
    }

    async changePassword(): Promise<void> {
        this.isFormProcessing = true;

        const { passwordConfirmation, ...rest } = this.changePasswordForm.getRawValue();

        try {
            await lastValueFrom(this.authService.changePassword({ ...rest }));
            this.isFormProcessing = false;
            this.onSubmit.emit();
        } catch (error) {
            this.isFormProcessing = false;
            this.initializeForm();
            console.error(error);
        }
    }
}
