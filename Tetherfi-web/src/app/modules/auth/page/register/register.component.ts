import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { IFormCustomClass } from '@data/schema/generic/form';

import { CustomValidators } from '@shared/util/custom-validators';

import { UserService } from '@data/service/user/user.service';
import { SweetAlertService } from '@app/service/sweet-alert.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
    registerForm: FormGroup;
    isFormProcessing = false;
    returnUrl: string;
    image: File[] = [];

    roles = [
        { role: 0, name: 'User' },
        { role: 1, name: 'Admin' },
    ];

    textInputCustomClasses: IFormCustomClass = {
        formGroup: 'form-group m-0 mb-2',
        label: 'col-form-label px-0 fs-14',
        input: 'form-control input-h-3 fs-14',
    };

    selectInputCustomClasses: IFormCustomClass = {
        formGroup: 'form-group',
        label: 'col col-form-label px-0 fs-14',
        input: 'input-h-3 fs-14',
    };

    constructor(
        private formBuilder: FormBuilder,
        private userService: UserService,
        private sweetAlertService: SweetAlertService,
        private router: Router,
    ) {
        this.initializeForm();
    }

    ngOnInit() {}

    private initializeForm(): void {
        this.registerForm = this.formBuilder.group(
            {
                firstName: ['', Validators.required],
                lastName: ['', Validators.required],
                username: ['', Validators.required],
                email: [
                    '',
                    [
                        Validators.required,
                        Validators.pattern(
                            /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                        ),
                    ],
                ],
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
                role: [0, Validators.required],
                dob: [null, Validators.required],
                profilePicture: [null],
            },
            {
                // check whether our password and confirm password match
                validator: CustomValidators.passwordMatchValidator,
            },
        );
    }

    get f() {
        return this.registerForm.controls;
    }

    register() {
        this.isFormProcessing = true;
        const { PasswordConfirmation, ...rest } = this.registerForm.value;
        this.userService.post({ ...rest }).subscribe({
            next: () => {
                this.registerForm.disable();
                this.isFormProcessing = false;
                this.sweetAlertService.success('Account creation successful!');
                this.router.navigate(['/auth/login']);
            },
            error: (error) => {
                this.isFormProcessing = false;
                console.error(error);
            },
        });
    }

    async onSelect($event): Promise<void> {
        if ($event.rejectedFiles.length > 0) {
            if ($event.rejectedFiles[0].reason === 'type')
                this.sweetAlertService.error('Please upload the following image types: .png, .jpg');
        } else {
            if ($event.rejectedFiles.length > 1) {
                $event.addFiles.shift();
            }
            this.image = [...$event.addedFiles];
            this.registerForm.patchValue({
                profilePicture: await this.toBase64(this.image[0]),
            });
        }
    }

    onRemove($event) {
        this.image.splice(this.image.indexOf($event), 1);
        this.registerForm.patchValue({
            profilePicture: null,
        });
    }

    toBase64(file: File){
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = reject;
        });
    }
}
