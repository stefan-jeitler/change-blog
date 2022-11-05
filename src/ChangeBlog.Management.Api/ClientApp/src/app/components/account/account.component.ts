import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TranslationKey} from "../../generated/TranslationKey";
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import {mergeMap} from "rxjs/operators";
import {Resource} from "../resource.state";
import Account = ChangeBlogManagementApi.AccountDto;

@Component({
    selector: 'app-account',
    templateUrl: './account.component.html',
    styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
    resource: Resource<Account>;

    constructor(private route: ActivatedRoute,
                public translationKey: TranslationKey,
                private apiClient: ChangeBlogManagementApi.Client) {
        this.resource = {state: 'loading'};
    }

    ngOnInit(): void {
        this.route.params
            .pipe(
                mergeMap(v => this.apiClient.getAccount(v.id)),
            )
            .subscribe({
                next: a => {
                    this.resource = {
                        state: 'success',
                        account: a
                    };
                },
                error: (e: ChangeBlogManagementApi.SwaggerException) => {
                    debugger;
                    if (e.status === 404) {
                        this.resource = {state: 'not-found'};
                    } else {
                        this.resource = {
                            state: 'unknown-error',
                            details: e.result?.errors ?? []
                        };
                    }

                    console.error(e);
                }
            });
    }

}
