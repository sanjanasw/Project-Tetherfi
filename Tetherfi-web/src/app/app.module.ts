import { HomeLayoutComponent } from './layouts/home-layout/home-layout.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AuthModule } from '@modules/auth/auth.module';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';

import { CoreModule } from '@app/core.module';
import { SharedModule } from './shared/shared.module';
import { SideNavComponent } from './layouts/side-nav/side-nav.component';
import { NavComponent } from './layouts/nav/nav.component';
import { MobileSideNavComponent } from './layouts/mobile-side-nav/mobile-side-nav.component';

@NgModule({
    declarations: [
        AppComponent,
        AuthLayoutComponent,
        SideNavComponent,
        MobileSideNavComponent,
        NavComponent,
        HomeLayoutComponent,
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        AuthModule,
        CoreModule,
        SharedModule,
        BrowserAnimationsModule,
    ],
    providers: [],
    bootstrap: [AppComponent],
})
export class AppModule {}
