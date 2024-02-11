import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { distinctUntilChanged } from 'rxjs/operators';

import { SCREEN_SIZE } from '@data/enums/screenSize';

@Injectable({
    providedIn: 'root',
})
export class ResizeService {
    private resizeBehaviorSubject: BehaviorSubject<SCREEN_SIZE>;

    constructor() {
        this.resizeBehaviorSubject = new BehaviorSubject(SCREEN_SIZE.LG);
    }

    get onResize$(): Observable<SCREEN_SIZE> {
        return this.resizeBehaviorSubject.asObservable().pipe(distinctUntilChanged());
    }

    onResize(size: SCREEN_SIZE) {
        this.resizeBehaviorSubject.next(size);
    }
}
