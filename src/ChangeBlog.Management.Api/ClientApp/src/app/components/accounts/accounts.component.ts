import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {ConfirmationService, MenuItem, MessageService} from "primeng/api";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {translate, TranslocoService} from "@ngneat/transloco";
import {
    ChangeBlogManagementApi as MngmtApiClient,
    ChangeBlogManagementApi
} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import UpdateAccountDto = ChangeBlogManagementApi.UpdateAccountDto;
import CreateAccountDto = ChangeBlogManagementApi.CreateAccountDto;

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
                private translationService: TranslocoService,
                private confirmationService: ConfirmationService,
                private messageService: MessageService,
                private mngmtApiClient: ChangeBlogManagementApi.Client) {
        this.isLoadingFinished = false;
        this.selectedAccounts = [];
        this.accounts = [];
        this.contextMenuItems = [];
        this.showDatatableLoadingOverlay = false;
        this.showAccountDialog = false;

        this.accountForm = formBuilder.group({
            id: new FormControl<string>(''),
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
            await this.loadAccounts();
        } finally {
            this.isLoadingFinished = true;
        }
    }

    async loadAccounts() {
        const accounts = await firstValueFrom(this.mngmtApiClient.getAccounts());
        this.accounts = accounts.map(x => {
            return {
                id: x.id,
                name: x.name!,
                defaultVersioningScheme: x.defaultVersioningScheme!,
                createdAt: x.createdAt,
                createdBy: x.createdBy!,
                wasCreatedByMyself: x.wasCreatedByMyself
            }
        });
    }

    createNewAccount() {
        this.accountForm.reset();
        this.accountForm.resetValidation()
        this.showAccountDialog = true;
    }

    updateAccount(account: Account) {
        this.accountForm.patchValue({
            id: account.id,
            name: account.name,
        });

        this.showAccountDialog = true;
    }

    onAccountSubmit() {
        this.accountForm.disable();

        let accountId = this.accountForm.value.id;

        const generateOrUpdateRequest = !!accountId
            ? this.mngmtApiClient.updateAccount(accountId, undefined, UpdateAccountDto.fromJS({name: this.accountForm.value.name}))
            : this.mngmtApiClient.createAccount(undefined, CreateAccountDto.fromJS({name: this.accountForm.value.name}));

        generateOrUpdateRequest
            .subscribe({
                next: async () => {
                    this.showDatatableLoadingOverlay = true;
                    this.accountForm.enable();
                    this.showAccountDialog = false;
                    await this.loadAccounts();
                    this.showDatatableLoadingOverlay = false;
                },
                error: async (error: MngmtApiClient.SwaggerException) => {
                    this.accountForm.enable();
                    await this.handleError(error);
                }
            });
    }

    closeAccountDialog() {
        this.showAccountDialog = false;
    }

    async deleteAccount(account: Account) {
        const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
        const confirmationQuestion = await firstValueFrom(this.translationService.selectTranslate(
            this.translationKey.confirmAccountDeletion,
            {accountName: account.name}));

        this.confirmationService.confirm({
            message: confirmationQuestion,
            header: title,
            icon: 'pi pi-exclamation-triangle',
            accept: async () => {
                this.showDatatableLoadingOverlay = true;
                await firstValueFrom(this.mngmtApiClient.deleteAccount(account.id));
                await this.loadAccounts();
                this.showDatatableLoadingOverlay = false;
            }
        });
    }

    private async handleError(error: MngmtApiClient.SwaggerException) {
        this.accountForm.enable();

        if (error.status >= 400 && error.status < 500) {
            this.accountForm.setErrors(error.result.errors);
            this.messageService.showGeneralErrors(error.result.errors);
        } else {
            await this.showGenericErrorMessage();
        }
    }

    private async showGenericErrorMessage() {
        const errorMessageHeader = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.genericErrorMessageShort));
        const errorMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.genericErrorMessage));

        const message = {severity: 'error', summary: errorMessageHeader, detail: errorMessage}
        this.messageService.add(message);
    }
}
