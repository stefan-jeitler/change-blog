import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {TranslationKey} from "../../generated/TranslationKey";
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import {Resource} from "../resource.state";
import {firstValueFrom} from "rxjs";
import ResourceType = ChangeBlogManagementApi.ResourceType;
import Permission = ChangeBlogManagementApi.ResourcePermissionsDto;

export type AccountTab = 'basic-data' | 'users' | 'products';

interface Tab {
    index: number;
    isActive: boolean;
}

@Component({
    selector: 'app-account',
    templateUrl: './account.component.html',
    styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
    resource: Resource<Permission>;
    tabs: { [tabName in AccountTab]: Tab };

    constructor(private route: ActivatedRoute,
                public translationKey: TranslationKey,
                private apiClient: ChangeBlogManagementApi.Client) {
        this.resource = {state: 'loading'};
        this.tabs = {
            'basic-data': {index: 0, isActive: false},
            'users': {index: 1, isActive: false},
            'products': {index: 2, isActive: false}
        };
    }

    initializeTabs(permission: Permission) {
        const accountPermissions = permission.specificPermissions ?? {};
        const canViewUsers = accountPermissions['canViewUsers'] ?? false;
        const canViewProducts = accountPermissions['canViewProducts'] ?? false;

        this.tabs = {
            'basic-data': {index: 0, isActive: true},
            'users': {index: 1, isActive: canViewUsers},
            'products': {index: 2, isActive: canViewProducts}
        };
    }

    async ngOnInit(): Promise<void> {

        try {
            const params = await firstValueFrom(this.route.params);

            firstValueFrom(this.apiClient.getPermissions(ResourceType.Account, params.id))
                .then(p => {
                    this.resource = {
                        state: 'success',
                        value: p
                    };

                    this.initializeTabs(p);
                })
                .catch((e: ChangeBlogManagementApi.SwaggerException) => {
                    if (e.status === 404)
                        this.resource = {state: 'not-found'};
                    else
                        this.resource = {state: 'error', errorDetails: e.result?.error ?? []};
                });

        } catch (e: any) {
            this.resource = {
                state: 'error',
                errorDetails: e?.result?.errors ?? []
            };

            console.error(e);
        }
    }
}
