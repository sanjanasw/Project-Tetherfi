/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { TextDisplayComponent } from './text-display.component';

describe('TextDisplayComponent', () => {
    let component: TextDisplayComponent;
    let fixture: ComponentFixture<TextDisplayComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [TextDisplayComponent],
        }).compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(TextDisplayComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
