import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { AuthGuard } from '@app/guard/auth.guard';
import { throwIfAlreadyLoaded } from '@app/guard/module-import.guard';

import { TokenInterceptor } from '@app/interceptor/token.interceptor';
import { ErrorInterceptor } from '@app/interceptor/error.interceptor';


@NgModule({
    declarations: [],
    imports: [],
    providers: [
        AuthGuard,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ErrorInterceptor,
            multi: true,
        },
    ],
})
export class CoreModule {
    constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
        throwIfAlreadyLoaded(parentModule, 'CoreModule');
    }
}
