// modules
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { BootstrapModule } from './bootstrap.module';
import { AvatarModule } from 'ngx-avatar';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxDropzoneModule } from 'ngx-dropzone';

// directives
import {
    NgSelectInputOptionTemplateDirective,
    NgSelectInputLabelTemplateDirective,
} from './directive/form-template.directive';

// components
import { PasswordInputComponent } from './components/forms/password-input/password-input.component';
import { SelectInputComponent } from './components/forms/select-input/select-input.component';
import { CheckboxInputComponent } from './components/forms/checkbox-input/checkbox-input.component';
import { SizeDetectorComponent } from './components/size-detector/size-detector.component';
import { TextInputComponent } from './components/forms/text-input/text-input.component';
import { TextDisplayComponent } from './components/forms/text-display/text-display.component';

// modals
import { ChangePasswordModalComponent } from './components/modals/change-password-modal/change-password-modal.component';
import { MyProfileModalComponent } from './components/modals/my-profile-modal/my-profile-modal.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        NgSelectModule,
        BootstrapModule,
        AvatarModule,
        NgxSpinnerModule,
        NgxDropzoneModule,
    ],
    declarations: [
        // components
        SizeDetectorComponent,
        PasswordInputComponent,
        TextInputComponent,
        TextDisplayComponent,
        CheckboxInputComponent,
        SelectInputComponent,

        // directives
        NgSelectInputOptionTemplateDirective,
        NgSelectInputLabelTemplateDirective,

        // modals
        ChangePasswordModalComponent,
        MyProfileModalComponent
    ],

    exports: [
        // core
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        BootstrapModule,
        AvatarModule,
        NgxSpinnerModule,
        NgxDropzoneModule,

        // components
        SizeDetectorComponent,
        TextInputComponent,
        TextDisplayComponent,
        PasswordInputComponent,
        CheckboxInputComponent,
        SelectInputComponent,

        // Directives
        NgSelectInputOptionTemplateDirective,
        NgSelectInputLabelTemplateDirective,

        // modals
        ChangePasswordModalComponent,
        MyProfileModalComponent
    ],
})
export class SharedModule {}
