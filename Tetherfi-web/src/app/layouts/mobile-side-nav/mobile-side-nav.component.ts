import { Component } from '@angular/core';
import { AuthService } from '@data/service/auth/auth.service';

import { LayoutService } from '@app/service/layout.service';

@Component({
    selector: 'app-mobile-side-nav',
    templateUrl: './mobile-side-nav.component.html',
    styleUrls: ['./mobile-side-nav.component.scss'],
})
export class MobileSideNavComponent {
    isMobileSidebarExpanded = true;

    constructor(public authService: AuthService, private LayoutService: LayoutService) {
        this.LayoutService.isMobileSidebarExpandedSource$.subscribe((isMobileExpanded) => {
            this.isMobileSidebarExpanded = isMobileExpanded;
        });
    }

    onCloseMobileSidebar() {
        this.isMobileSidebarExpanded = !this.isMobileSidebarExpanded;
        this.LayoutService.toggleMobileSidebarExpandedState(this.isMobileSidebarExpanded);
    }
}
