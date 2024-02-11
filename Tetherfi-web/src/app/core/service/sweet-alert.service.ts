import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

import { AuthService } from '@data/service/auth/auth.service';

@Injectable({
    providedIn: 'root',
})
export class SweetAlertService {
    toast = Swal.mixin({
        toast: true,
        position: 'bottom',
        showConfirmButton: false,
        customClass: {
            title: 'alert-title',
            icon: 'alert-icon',
        },
        timer: 6000,
        timerProgressBar: false,
    });

    constructor(private router: Router, private authService: AuthService) {}

    success(message: string) {
        this.toast.fire({
            icon: 'success',
            background: '#D9FDEA',
            title: message,
            iconColor: '#43C882',
            customClass: {
                popup: 'alert-popup-success',
                title: 'alert-title',
                icon: 'alert-icon',
            },
        });
    }

    error(message: string) {
        this.toast.fire({
            icon: 'error',
            background: '#FFF1F0',
            title: message,
            iconColor: '#FF4D4F',
            customClass: {
                popup: 'alert-popup-error',
                title: 'alert-title',
                icon: 'alert-icon',
            },
        });
    }

    warning(message: string) {
        this.toast.fire({
            icon: 'warning',
            background: '#FFFBE6',
            title: message,
            iconColor: '#FAAD14',
            customClass: {
                popup: 'alert-popup-warning',
                title: 'alert-title',
                icon: 'alert-icon',
            },
        });
    }

    message(message: string) {
        this.toast.fire({
            icon: 'info',
            background: '#E6F7FF',
            title: message,
            iconColor: '#1890FF',
            customClass: {
                popup: 'alert-popup-info',
                title: 'alert-title',
                icon: 'alert-icon',
            },
        });
    }

    unauthorized(message: string) {
        this.authService.logout();
        this.router.navigate(['auth/login'], {
            queryParams: { returnUrl: this.router.url },
        });
        this.toast.fire({
            icon: 'warning',
            title: message,
        });
    }

    processing(message: string = 'Processing') {
        Swal.fire({
            html: message,
            didOpen: () => {
                Swal.showLoading();
            },
        });
    }
}
