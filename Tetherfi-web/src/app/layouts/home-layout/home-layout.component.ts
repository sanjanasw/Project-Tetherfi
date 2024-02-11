import { Component, OnInit } from '@angular/core';
import { delay } from 'rxjs/operators';

import { LayoutService } from '@app/service/layout.service';
import { ResizeService } from '@app/service/resize.service';

import { SCREEN_SIZE } from '@data/enums/screenSize';

@Component({
    selector: 'app-home-layout',
    templateUrl: './home-layout.component.html',
    styleUrls: ['./home-layout.component.scss'],
})
export class HomeLayoutComponent implements OnInit {
    isSidebarExpanded = true;
    screenSize: SCREEN_SIZE;
    constructor(private layoutService: LayoutService, private resizeService: ResizeService) {
        this.resizeService.onResize$.pipe(delay(0)).subscribe((size) => {
            this.screenSize = size;
            this.layoutService.toggleSidebarExpandedState(this.screenSize < 4 ? false : true);
        });
    }

    ngOnInit(): void {
        this.layoutService.isSidebarExpanded$.subscribe((isExpanded) => {
            this.isSidebarExpanded = isExpanded;
        });
    }
}
