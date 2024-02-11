import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '@data/service/auth/auth.service';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.authService.currentUser$.pipe(
            map((res) => {
                if (res?.user) {
                    const redirectUrl = '/home';
                    // check if route is restricted by role
                    if (route.data.roles && route.data.roles.indexOf(res.user.roles[0]) === -1) {
                        // role not authorized so redirect to home page
                        this.router.navigate([redirectUrl]);
                        return false;
                    }

                    return true;
                }
                this.router.navigate(['auth/login'], { queryParams: { returnUrl: state.url } });
                return false;
            }),
        );
    }
}
