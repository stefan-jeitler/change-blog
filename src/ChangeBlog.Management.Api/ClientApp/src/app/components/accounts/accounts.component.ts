import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {MessageService} from "primeng/api";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {TranslocoService} from "@ngneat/transloco";
import {ChangeBlogManagementApi as MngmtApiClient} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {Router} from "@angular/router";
import CreateAccountDto = MngmtApiClient.CreateAccountDto;

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
    readonly maxAccountsCreatedByMyself: number = 5;
    showDatatableLoadingOverlay: boolean;
    showAccountDialog: boolean;
    accountForm: FormGroup;


    constructor(public translationKey: TranslationKey,
                private formBuilder: FormBuilder,
                private translationService: TranslocoService,
                private messageService: MessageService,
                private mngmtApiClient: MngmtApiClient.Client,
                private router: Router) {
        this.isLoadingFinished = false;
        this.selectedAccounts = [];
        this.accounts = [];
        this.showDatatableLoadingOverlay = false;
        this.showAccountDialog = false;

        this.accountForm = formBuilder.group({
            id: new FormControl<string>(''),
            name: new FormControl<string>('')
        });

    }

    get ownCreatedAccountsCount(): number {
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

    async openAccount(account: Account) {
        await this.router.navigate(['/app/accounts', account.id]);
    }

    onAccountFormSubmit() {
        this.accountForm.disable();
        const accountDto = CreateAccountDto.fromJS({name: this.accountForm.value.name});
        const createAccount = this.mngmtApiClient.createAccount(undefined, accountDto);

        createAccount
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
