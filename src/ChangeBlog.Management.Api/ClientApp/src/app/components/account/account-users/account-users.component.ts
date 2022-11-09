import {Component, Input, OnInit} from '@angular/core';
import {TranslationKey} from "../../../generated/TranslationKey";
import {Resource} from "../../resource.state";
import {ChangeBlogManagementApi} from "../../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import User = ChangeBlogManagementApi.UserDto;

@Component({
    selector: 'app-account-users',
    templateUrl: './account-users.component.html',
    styleUrls: ['./account-users.component.scss']
})
export class AccountUsersComponent implements OnInit {
    @Input() accountId: string;
    resource: Resource<User[]>

    constructor(public translationKey: TranslationKey,
                private apiClient: ChangeBlogManagementApi.Client) {
        this.accountId = '';
        this.resource = {state: 'loading'};
    }

    ngOnInit(): void {
        this.loadUsers();
    }

    loadUsers() {
        firstValueFrom(this.apiClient.getAccountUsers(this.accountId))
            .then(u => {
                this.resource = {
                    state: 'success',
                    value: u
                };
            })
            .catch((e: ChangeBlogManagementApi.SwaggerException) => {
                if (e.status === 404)
                    this.resource = {state: 'not-found'};
                else
                    this.resource = {
                        state: 'error',
                        errorDetails: e.result?.errors ?? []
                    }
            })
    }

    getValue(event: Event): string {
        return (event.target as HTMLInputElement).value;
    }
}
