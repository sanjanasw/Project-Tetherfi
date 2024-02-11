import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import {
    ILoginResult,
    IHttpResult,
    ILoginContext,
    IRefreshTokenContext,
    IChangePasswordContext,
} from '@data/schema/user';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    baseUrl: string;

    private currentUserSource = new BehaviorSubject<ILoginResult>(null);
    currentUser$ = this.currentUserSource.asObservable();

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    login(loginContext: ILoginContext) {
        return this.http.post<IHttpResult<ILoginResult>>(this.baseUrl + '/auth/login', loginContext).pipe(
            map((res: IHttpResult<ILoginResult>) => {
                if (res.Result) {
                    this.setCurrentUser(res.Result);
                }
                return res.Result;
            }),
        );
    }

    refreshToken(refreshTokenContext: IRefreshTokenContext) {
        return this.http.post(this.baseUrl + '/auth/refresh-token', refreshTokenContext).pipe(
            map((res: IHttpResult<ILoginResult>) => {
                if (res.Result) {
                    this.setCurrentUser(res.Result);
                }
                return res.Result;
            }),
        );
    }

    changePassword(changePasswordContext: IChangePasswordContext) {
        return this.http.put(this.baseUrl + '/auth/change-password', changePasswordContext);
    }

    logout() {
        localStorage.clear();
        this.currentUserSource.next(null);
    }

    setCurrentUser(res: ILoginResult) {
        if (this.isTokenExpired(res)) {
            return this.logout();
        }

        localStorage.setItem('loginResult', JSON.stringify(res));
        this.currentUserSource.next(res);
    }

    private isTokenExpired(user: ILoginResult) {
        if (!user || !user?.token) {
            return true;
        }

        // parse json object from base64 encoded jwt token
        const jwtToken = JSON.parse(atob(user.token.split('.')[1]));

        const expires = new Date(jwtToken.exp * 1000);
        return expires <= new Date();
    }
}
