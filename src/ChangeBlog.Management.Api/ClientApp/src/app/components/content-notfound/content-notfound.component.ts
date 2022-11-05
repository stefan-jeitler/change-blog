import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-content-notfound',
    templateUrl: './content-notfound.component.html',
    styleUrls: ['./content-notfound.component.scss']
})
export class ContentNotfoundComponent implements OnInit {

    @Input() resourceId: string;

    constructor() {
        this.resourceId = '';
    }

    ngOnInit(): void {
    }

}
