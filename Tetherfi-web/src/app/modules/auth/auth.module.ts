import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '@shared/shared.module';

import { LoginComponent } from './page/login/login.component';
import { AuthRoutingModule } from './auth-routing.module';
import { RegisterComponent } from './page/register/register.component';

@NgModule({
    imports: [CommonModule, AuthRoutingModule, SharedModule],
    declarations: [LoginComponent, RegisterComponent],
})
export class AuthModule {}
