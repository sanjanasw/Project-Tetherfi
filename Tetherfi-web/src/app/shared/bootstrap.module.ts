import { NgModule } from '@angular/core';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@NgModule({
    declarations: [],
    imports: [BsDropdownModule.forRoot(), CollapseModule.forRoot(), ModalModule.forRoot(), ButtonsModule.forRoot()],
    exports: [BsDropdownModule, CollapseModule, ModalModule, ButtonsModule],
})
export class BootstrapModule {}
