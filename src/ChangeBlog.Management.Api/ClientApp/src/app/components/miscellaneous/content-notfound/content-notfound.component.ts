import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../../generated/TranslationKey";

@Component({
    selector: 'app-content-notfound',
    templateUrl: './content-notfound.component.html',
    styleUrls: ['./content-notfound.component.scss']
})
export class ContentNotfoundComponent implements OnInit {

    constructor(public translationKey: TranslationKey) {
    }

    ngOnInit(): void {
    }

}
