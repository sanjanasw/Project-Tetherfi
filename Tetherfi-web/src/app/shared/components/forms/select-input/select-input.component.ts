import { Component, Input, Self, Output, EventEmitter, ContentChild, TemplateRef } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

import {
    NgSelectInputOptionTemplateDirective,
    NgSelectInputLabelTemplateDirective,
} from '@shared/directive/form-template.directive';

import { IFormCustomClass } from '@data/schema/generic/form';

@Component({
    selector: 'app-select-input',
    templateUrl: './select-input.component.html',
    styleUrls: ['./select-input.component.scss'],
})
export class SelectInputComponent implements ControlValueAccessor {
    @Input() label: string;
    @Input() placeholder: string;
    @Input() list: any[];
    @Input() labelField: string;
    @Input() bindField: string;
    @Input() clearable = false;
    @Input() closeOnSelect = true;
    @Input() multiple = false;
    @Input() appendTo: string;
    @Input() showRequiredIndicator = false;
    @Input() searchable = false;
    @Input() addTag = false;
    @Input() addTagText = '+';
    @Input() customClasses: IFormCustomClass = {
        formGroup: 'form-group',
        label: 'font-weight-bold text-muted small mb-0',
        input: '',
    };
    @Output() onOptionSelect = new EventEmitter();
    @Output() onOptionClear = new EventEmitter();

    // custom templates
    @ContentChild(NgSelectInputOptionTemplateDirective, { read: TemplateRef }) optionTemplate: TemplateRef<any>;
    @ContentChild(NgSelectInputLabelTemplateDirective, { read: TemplateRef }) labelTemplate: TemplateRef<any>;

    constructor(@Self() public ngControl: NgControl) {
        this.ngControl.valueAccessor = this;
    }
    writeValue(obj: any): void {}
    registerOnChange(fn: any): void {}
    registerOnTouched(fn: any): void {}
    setDisabledState?(isDisabled: boolean): void {}

    onChange($event: any) {
        this.onOptionSelect.emit($event);
    }

    onClear() {
        this.onOptionClear.emit();
    }
}
