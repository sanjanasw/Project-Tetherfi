import { Directive, TemplateRef } from '@angular/core';

@Directive({ selector: '[ng-select-input-option-tmp]' })
export class NgSelectInputOptionTemplateDirective {
    constructor(public template: TemplateRef<any>) {}
}

@Directive({ selector: '[ng-select-input-label-tmp]' })
export class NgSelectInputLabelTemplateDirective {
    constructor(public template: TemplateRef<any>) {}
}
