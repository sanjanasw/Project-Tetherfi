import { Component, OnInit } from '@angular/core';

import { AuthService } from '@data/service/auth/auth.service';
import { LayoutService } from '@app/service/layout.service';

@Component({
    selector: 'app-side-nav',
    templateUrl: './side-nav.component.html',
    styleUrls: ['./side-nav.component.scss'],
})
export class SideNavComponent implements OnInit {
    isSidebarExpanded = true;

    constructor(public authService: AuthService, private layoutService: LayoutService) {}

    ngOnInit(): void {
        this.layoutService.isSidebarExpanded$.subscribe((isExpanded) => {
            this.isSidebarExpanded = isExpanded;
        });
    }

    toggleSidebarExpand() {
        this.isSidebarExpanded = !this.isSidebarExpanded;
        this.layoutService.toggleSidebarExpandedState(this.isSidebarExpanded);
    }
}
