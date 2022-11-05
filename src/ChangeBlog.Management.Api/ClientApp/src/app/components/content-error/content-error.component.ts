import {Component, Input, OnInit} from '@angular/core';
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import ErrorMessages = ChangeBlogManagementApi.ErrorMessages;

@Component({
    selector: 'app-content-error',
    templateUrl: './content-error.component.html',
    styleUrls: ['./content-error.component.scss']
})
export class ContentErrorComponent implements OnInit {

    @Input() details: ErrorMessages[];

    constructor() {
        this.details = []
    }

    ngOnInit(): void {
    }

}
