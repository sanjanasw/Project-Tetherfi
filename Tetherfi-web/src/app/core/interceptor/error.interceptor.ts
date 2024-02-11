import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { catchError, finalize } from 'rxjs/operators';

import { SweetAlertService } from '@app/service/sweet-alert.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private router: Router,
        private sweetAlertService: SweetAlertService,
        private ngxSpinnerService: NgxSpinnerService,
    ) {}

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        this.ngxSpinnerService.show();
        return next.handle(request).pipe(
            finalize(() => this.ngxSpinnerService.hide('commonSpinner')),
            catchError((error) => {
                if (error) {
                    switch (error.status) {
                        case 400:
                        case 422:
                            if (error.error.Result.errors) {
                                const modalStateErrors = [];
                                for (const key in error.error.Result.errors) {
                                    if (error.error.Result.errors[key]) {
                                        modalStateErrors.push(error.error.Result.errors[key]);
                                    }
                                }
                                this.sweetAlertService.error(modalStateErrors.flat().toString());
                                throw modalStateErrors.flat();
                            } else if (typeof error.error === 'object') {
                                if (typeof error.error.error === 'object') {
                                    this.sweetAlertService.error(error.error.error.message);
                                } else {
                                    this.sweetAlertService.error(error.error.message);
                                }
                            } else {
                                this.sweetAlertService.error(error.error);
                            }
                            break;
                        case 401:
                            this.sweetAlertService.unauthorized(error.error.Result);
                            break;
                        case 403:
                            this.sweetAlertService.unauthorized("User doesn't have permission to perform this action");
                            break;
                        case 404:
                            this.router.navigateByUrl('/not-found');
                            break;
                        case 500:
                            this.sweetAlertService.error(error.error.message);
                            break;
                        default:
                            this.sweetAlertService.error('Something went wrong');
                            console.log(error);
                            break;
                    }
                }
                return throwError(error);
            }),
        );
    }
}
