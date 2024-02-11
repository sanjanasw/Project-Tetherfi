import { Component, EventEmitter, Input, Output, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { IFormCustomClass } from '@data/schema/generic/form';

@Component({
    selector: 'app-text-input',
    templateUrl: './text-input.component.html',
    styleUrls: ['./text-input.component.scss'],
})
export class TextInputComponent implements ControlValueAccessor {
    @Input() label: string;
    @Input() placeholder: string;
    @Input() type = 'text';
    @Input() step: number;
    @Input() inputGroupAppendEnabled = false;
    @Input() inputGroupAppendText: string;
    @Input() inputGroupAppendButtonText: string;
    @Input() inputGroupAppendButtonProcessing = false;
    @Input() inputGroupAppendButtonDisabled = false;
    @Input() inputDecimalCheckEnabled = false;
    @Input() inputDecimalPattern = new RegExp('^\\d+(\\.\\d{1,2})?$');
    @Input() appendButtonClasses: string;
    @Input() elementId: string;
    @Input() customClasses: IFormCustomClass = {
        formGroup: 'form-group',
        label: 'font-weight-bold text-muted small mb-0',
        input: 'form-control',
    };
    @Input() showRequiredIndicator: boolean;
    txtValue = '';
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
}
