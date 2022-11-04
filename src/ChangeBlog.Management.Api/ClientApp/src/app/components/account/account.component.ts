import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TranslationKey} from "../../generated/TranslationKey";

@Component({
    selector: 'app-account',
    templateUrl: './account.component.html',
    styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
    isLoadingFinished: boolean;
    accountId: string | undefined;

    constructor(private route: ActivatedRoute,
                public translationKey: TranslationKey) {
        this.isLoadingFinished = false;
    }

    ngOnInit(): void {
        this.route.params.subscribe({
            next: v => {
                this.accountId = v.id;
            }
        });

        this.isLoadingFinished = true;
    }

}
