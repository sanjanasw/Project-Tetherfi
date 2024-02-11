import { Component, OnInit } from '@angular/core';
import { AuthService } from '@data/service/auth/auth.service';
import { ILoginResult } from '@data/schema/user';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
    title = 'Tetherfi-app';

    constructor(private authService: AuthService) {}

    ngOnInit() {
        this.setCurrentUser();
    }

    setCurrentUser() {
        const user: ILoginResult = JSON.parse(localStorage.getItem('loginResult'));
        if (user) {
            this.authService.setCurrentUser(user);
        } else {
            this.authService.setCurrentUser(null);
        }
    }
}
