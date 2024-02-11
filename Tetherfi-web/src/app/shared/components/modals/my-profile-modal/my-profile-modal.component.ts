import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SweetAlertService } from '@app/service/sweet-alert.service';
import { IFormCustomClass } from '@data/schema/generic/form';
import { ILoginResult, IUser } from '@data/schema/user';
import { AuthService } from '@data/service/auth/auth.service';
import { UserService } from '@data/service/user/user.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { first, lastValueFrom } from 'rxjs';

@Component({
    selector: 'app-my-profile-modal',
    templateUrl: './my-profile-modal.component.html',
    styleUrls: ['./my-profile-modal.component.scss'],
})
export class MyProfileModalComponent implements OnInit {
    @Output() onSubmit = new EventEmitter();
    myProfileForm: FormGroup;
    isFormProcessing = false;
    currentUser: IUser;

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
        public bsModalRef: BsModalRef,
        private sweetAlertService: SweetAlertService,
        private authService: AuthService,
        private userService: UserService,
    ) {
        this.authService.currentUser$.pipe(first()).subscribe((res: ILoginResult) => {
            if (res) {
                this.currentUser = res.user;
            }
        });
    }

    ngOnInit() {
        this.initializeForm();
        this.patchForm();
    }

    onHideClick() {
        this.bsModalRef.hide();
    }

    private initializeForm(): void {
        this.myProfileForm = this.formBuilder.group({
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
            dob: [null, Validators.required],
            profilePicture: [null],
        });
    }

    private patchForm(): void {
        this.myProfileForm.patchValue({
            ...this.currentUser,
        });
        this.image = [];
        this.image.push(this.base64toFile(this.currentUser.profilePicture, this.currentUser.id));
    }

    get f() {
        return this.myProfileForm.controls;
    }

    async myProfile(): Promise<void> {
        this.isFormProcessing = true;

        try {
            await lastValueFrom(this.userService.put(this.myProfileForm.getRawValue(), this.currentUser.id));
            this.isFormProcessing = false;
            this.onSubmit.emit();
        } catch (error) {
            this.isFormProcessing = false;
            this.initializeForm();
            this.patchForm();
            console.error(error);
        }
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
            this.myProfileForm.patchValue({
                profilePicture: await this.toBase64(this.image[0]),
            });
        }
    }

    onRemove($event) {
        this.image.splice(this.image.indexOf($event), 1);
        this.myProfileForm.patchValue({
            profilePicture: null,
        });
    }

    toBase64(file: File) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = reject;
        });
    }

    base64toFile(base64String, filename) {
        // Split the base64 string into two parts
        const parts = base64String.split(';base64,');
        const contentType = parts[0].split(':')[1];
        const raw = window.atob(parts[1]);
        const rawLength = raw.length;

        // Convert the raw binary data to a Uint8Array
        const array = new Uint8Array(new ArrayBuffer(rawLength));

        for (let i = 0; i < rawLength; ++i) {
            array[i] = raw.charCodeAt(i);
        }

        // Create a Blob from the binary data
        const blob = new Blob([array], { type: contentType });

        // Amend file extension from base64
        const extension = contentType.split('/')[1];
        const amendedFilename = `${filename}.${extension}`;

        // Create a File object from the Blob with the amended filename
        return new File([blob], amendedFilename, { type: contentType });
    }
}
