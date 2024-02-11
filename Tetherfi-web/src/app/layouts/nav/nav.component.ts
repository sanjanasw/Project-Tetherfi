import { Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

import { AuthService } from '@data/service/auth/auth.service';
import { LayoutService } from '@app/service/layout.service';
import { SweetAlertService } from '@app/service/sweet-alert.service';

import { ILoginResult } from '@data/schema/user';

import { ChangePasswordModalComponent } from '@shared/components/modals/change-password-modal/change-password-modal.component';
import { MyProfileModalComponent } from '@shared/components/modals/my-profile-modal/my-profile-modal.component';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit{
    currentUser: any;
    bsChangePasswordModalRef: BsModalRef;
    bsMyProfileModalRef: BsModalRef;
    dropDownIsOpen = false;
    isNavMenuCollapsed = true;
    isMobileSidebarExpanded = true;

    constructor(
        public authService: AuthService,
        private router: Router,
        private layoutService: LayoutService,
        private modalService: BsModalService,
        private sweetAlertService: SweetAlertService,
    ) {
        this.layoutService.isMobileSidebarExpandedSource$.subscribe((isMobileExpanded) => {
            this.isMobileSidebarExpanded = isMobileExpanded;
        });

        this.authService.currentUser$.pipe(first()).subscribe((res: ILoginResult) => {
            if (res) {
                this.currentUser = res.user;
            }
        });

    }

    ngOnInit(): void {}

    logout() {
        this.authService.logout();
        this.router.navigate(['auth/login']);
    }

    onNavbarTogglerClick() {
        this.isNavMenuCollapsed = !this.isNavMenuCollapsed;
    }

    onMobileNavToggleClick() {
        this.isMobileSidebarExpanded = false;
        this.layoutService.toggleMobileSidebarExpandedState(this.isMobileSidebarExpanded);
    }

    onOverlayClose() {
        this.isMobileSidebarExpanded = !this.isMobileSidebarExpanded;
        this.layoutService.toggleMobileSidebarExpandedState(this.isMobileSidebarExpanded);
    }

    onIsOpen() {
        this.dropDownIsOpen ? (this.dropDownIsOpen = false) : (this.dropDownIsOpen = true);
    }

    changePassword() {
        this.bsChangePasswordModalRef = this.modalService.show(ChangePasswordModalComponent, {
            class: 'modal-dialog-centered',
            backdrop: 'static',
        });
        this.bsChangePasswordModalRef.content.onSubmit.subscribe(() => {
            this.sweetAlertService.success('Password changed!');
            this.bsChangePasswordModalRef.hide();
        });
    }

    myProfile() {
        this.bsMyProfileModalRef = this.modalService.show(MyProfileModalComponent, {
            class: 'modal-dialog-centered',
            backdrop: 'static',
        });
        this.bsMyProfileModalRef.content.onSubmit.subscribe(() => {
            this.sweetAlertService.success('Profile Updated!');
            this.bsMyProfileModalRef.hide();
        });
    }
}
