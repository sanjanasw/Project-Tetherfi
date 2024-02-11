import { Component, EventEmitter, Input, Output, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { IFormCustomClass } from '@data/schema/generic/form';

@Component({
    selector: 'app-password-input',
    templateUrl: './password-input.component.html',
    styleUrls: ['./password-input.component.scss'],
})
export class PasswordInputComponent implements ControlValueAccessor {
    @Input() label: string;
    @Input() placeholder: string;
    @Input() isPasswordConfirm = false;

    @Input() customClasses: IFormCustomClass = {
        formGroup: 'form-group',
        label: 'font-weight-bold text-muted small mb-0',
        input: 'form-control',
    };
    txtValue = '';
    isTouched = false;
    isError = false;
    imageWarning = '/assets/images/common/warning.png';
    imageSuccess = '/assets/images/common/success.png';

    @Output() onAppendButtonClick = new EventEmitter();

    constructor(@Self() public ngControl: NgControl) {
        this.ngControl.valueAccessor = this;
    }

    writeValue(obj: any): void {}

    registerOnChange(fn: any): void {}

    registerOnTouched(fn: any): void {}

    onAppendClick() {
        this.onAppendButtonClick.emit(true);
    }

    onTextChange(value) {
        this.txtValue = value;
    }

    onKeyUpEvent(event: any) {
        this.txtValue = event.target.value;
    }

    focusOn() {
        this.isTouched = true;
    }

    onBlur() {
        this.txtValue.length ? (this.isTouched = true) : (this.isTouched = false);
        this.isError =
            !this.ngControl.control.errors?.required && this.ngControl.control.errors && !this.isPasswordConfirm;
    }
}
