import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '@data/service/auth/auth.service';

import { IFormCustomClass } from '@data/schema/generic/form';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    isFormProcessing = false;
    returnUrl: string;

    textInputCustomClasses: IFormCustomClass = {
        formGroup: 'form-group m-0 mb-2',
        label: 'col-sm-5 col-form-label px-0 fs-14',
        input: 'form-control input-h-3 fs-14',
    };

    checkboxCustomClasses: IFormCustomClass = {
        formGroup: 'form-group m-0',
        label: 'font-weight-bold text-muted small mb-0 fs-14 d-flex',
        input: 'form-check-input me-2',
    };

    isLogged = false;

    constructor(private formBuilder: FormBuilder, private authService: AuthService, private router: Router) {
        this.initializeForm();
    }

    ngOnInit() {
        this.authService.logout();
    }

    private initializeForm(): void {
        this.loginForm = this.formBuilder.group({
            username: ['', [Validators.required]],
            password: ['', Validators.required],
            rememberMe: [false],
        });
    }

    get f() {
        return this.loginForm.controls;
    }

    login() {
        this.isFormProcessing = true;
        this.authService.login(this.loginForm.value).subscribe({
            next: () => {
                this.loginForm.disable();
                this.isFormProcessing = false;
                this.isLogged = true;
                this.router.navigate(['/home']);
            },
            error: (error) => {
                this.isFormProcessing = false;
                console.error(error);
            },
        });
    }
}
