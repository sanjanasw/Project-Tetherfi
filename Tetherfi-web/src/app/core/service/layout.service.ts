import { Injectable } from '@angular/core';
import { BehaviorSubject} from 'rxjs';


@Injectable({
    providedIn: 'root',
})
export class LayoutService {
    private isMobileSidebarExpandedSource = new BehaviorSubject<boolean>(true);
    isMobileSidebarExpandedSource$ = this.isMobileSidebarExpandedSource.asObservable();

    private isSidebarExpandedSource = new BehaviorSubject<boolean>(true);
    isSidebarExpanded$ = this.isSidebarExpandedSource.asObservable();

    constructor() {}

    toggleMobileSidebarExpandedState(isMobileExpanded?: boolean) {
        this.isMobileSidebarExpandedSource.next(isMobileExpanded);
    }

    toggleSidebarExpandedState(isExpanded?: boolean) {
        this.isSidebarExpandedSource.next(isExpanded);
    }
}
