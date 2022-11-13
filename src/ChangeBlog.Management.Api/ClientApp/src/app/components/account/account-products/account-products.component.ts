import {Component, Input, OnInit} from '@angular/core';
import {Resource} from "../../resource.state";
import {TranslationKey} from "../../../generated/TranslationKey";
import {firstValueFrom} from "rxjs";
import {ChangeBlogManagementApi} from "../../../../clients/ChangeBlogManagementApiClient";
import Product = ChangeBlogManagementApi.ProductDto;

@Component({
    selector: 'app-account-products',
    templateUrl: './account-products.component.html',
    styleUrls: ['./account-products.component.scss']
})
export class AccountProductsComponent implements OnInit {

    @Input() accountId: string;
    resource: Resource<Product[]>

    constructor(public translationKey: TranslationKey,
                private apiClient: ChangeBlogManagementApi.Client) {
        this.accountId = '';
        this.resource = {state: 'loading'};
        this.accountId = '';
    }

    ngOnInit(): void {
        this.loadUsers();
    }

    loadUsers() {
        firstValueFrom(this.apiClient.getAccountProducts(this.accountId, undefined, undefined, undefined, true))
            .then(p => {
                this.resource = {
                    state: 'loaded',
                    value: p
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
