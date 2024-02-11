import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize, take } from 'rxjs/operators';

import { AuthService } from '@data/service/auth/auth.service';
import { ILoginResult } from '@data/schema/user';

import { NgxSpinnerService } from 'ngx-spinner';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService, private ngxSpinnerService: NgxSpinnerService) {}

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        let currentUser: ILoginResult;
        this.ngxSpinnerService.show();
        this.authService.currentUser$.pipe(take(1)).subscribe((user) => (currentUser = user));
        this.ngxSpinnerService.show('commonSpinner');
        if (currentUser) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${currentUser.token}`,
                },
            });
        }

        return next.handle(request).pipe(finalize(() => this.ngxSpinnerService.hide('commonSpinner')));
    }
}
