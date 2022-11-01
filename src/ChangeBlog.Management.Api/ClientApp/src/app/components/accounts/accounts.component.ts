import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {MenuItem} from "primeng/api";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {translate} from "@ngneat/transloco";
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";

interface Account {
    id: string;
    name: string,
    defaultVersioningScheme: string;
    createdAt: Date;
    createdBy: string;
    wasCreatedByMyself: boolean;
}

@Component({
    selector: 'app-accounts',
    templateUrl: './accounts.component.html',
    styleUrls: ['./accounts.component.scss']
})
export class AccountsComponent implements OnInit {
    isLoadingFinished: boolean;
    selectedAccounts: Account[];
    accounts: Account[];
    actionMenuTarget: Account | undefined;
    contextMenuItems: MenuItem[];
    readonly maxAccountsCreatedByMyselfLimit: number = 5;
    showDatatableLoadingOverlay: boolean;
    showAccountDialog: boolean;
    accountForm: FormGroup;


    constructor(public translationKey: TranslationKey,
                formBuilder: FormBuilder,
                private mngmtApiClient: ChangeBlogManagementApi.Client) {
        this.isLoadingFinished = false;
        this.selectedAccounts = [];
        this.accounts = [];
        this.contextMenuItems = [];
        this.showDatatableLoadingOverlay = false;
        this.showAccountDialog = false;

        this.accountForm = formBuilder.group({
            name: new FormControl<string>('')
        });

        this.contextMenuItems = [
            {
                label: translate(this.translationKey.edit),
                command: async () => {
                    if (!!this.actionMenuTarget)
                        await this.updateAccount(this.actionMenuTarget);
                },
                icon: 'pi pi-fw pi-pencil'
            },
            {
                label: translate(this.translationKey.delete),
                command: async () => {
                    if (!!this.actionMenuTarget)
                        await this.deleteAccount(this.actionMenuTarget);
                },
                icon: 'pi pi-fw pi-trash'
            }];

    }

    get maxOwnCreatedAccountsCount(): number {
        return this.accounts.filter(x => x.wasCreatedByMyself).length;
    }

    getValue(event: Event): string {
        return (event.target as HTMLInputElement).value;
    }

    async ngOnInit() {
        try {
            const accounts = await firstValueFrom(this.mngmtApiClient.getAccounts());
            this.accounts = accounts.map(x => {
                return {
                    id: x.id,
                    name: x.name ?? '',
                    defaultVersioningScheme: x.defaultVersioningScheme ?? '',
                    createdAt: x.createdAt,
                    createdBy: x.createdBy ?? '',
                    wasCreatedByMyself: x.wasCreatedByMyself
                }
            });
        } finally {
            this.isLoadingFinished = true;
        }
    }

    createNewAccount() {

    }

    deleteSelectedAccounts() {

    }

    updateAccount(account: Account) {

    }

    onAccountSubmit(accountForm: FormGroup) {

    }

    closeAccountDialog() {

    }

    deleteAccount(account: Account) {

    }
}
