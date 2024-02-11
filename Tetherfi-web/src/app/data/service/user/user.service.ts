import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IRegisterUser, IUserUpdate } from '@data/schema/user';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    baseUrl: string;
    currentUserSchoolId: number;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    post(user: IRegisterUser) {
        return this.http.post(this.baseUrl + '/user', user);
    }

    put(user: IUserUpdate, id: string) {
        return this.http.put(this.baseUrl + `/user/${id}`, user);
    }
}
