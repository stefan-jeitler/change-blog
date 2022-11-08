import {Component, Input, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {Resource} from "../resource.state";
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {MessageService} from "primeng/api";
import {TranslocoService} from "@ngneat/transloco";
import Account = ChangeBlogManagementApi.AccountDto;
import UpdateAccountDto = ChangeBlogManagementApi.UpdateAccountDto;

@Component({
    selector: 'app-account-master-data',
    templateUrl: './account-master-data.component.html',
    styleUrls: ['./account-master-data.component.scss']
})
export class AccountMasterDataComponent implements OnInit {

    @Input() accountId: string;
    @Input() readonly: boolean;
    resource: Resource<Account>;
    accountForm: FormGroup;

    constructor(public translationKey: TranslationKey,
                private formBuilder: FormBuilder,
                private apiClient: ChangeBlogManagementApi.Client,
                private messageService: MessageService,
                private translationService: TranslocoService) {
        this.accountId = '';
        this.readonly = true;
        this.resource = {state: 'loading'};

        this.accountForm = formBuilder.group({
            name: new FormControl<string>('')
        });
    }

    async ngOnInit(): Promise<void> {
        if (this.readonly)
            this.accountForm.disable();

        await this.loadAccount();
    }

    updateAccount() {
        this.accountForm.resetValidation();
        this.accountForm.disable();

        const dto = UpdateAccountDto.fromJS({
            name: this.accountForm.value.name
        });

        this.apiClient.updateAccount(this.accountId, undefined, dto)
            .subscribe({
                next: r => {
                    this.messageService.add({
                        severity: 'success',
                        detail: r.message
                    });
                },
                error: async (error: ChangeBlogManagementApi.SwaggerException) => {
                    await this.handleError(error);
                },
                complete: () => this.accountForm.enable()
            });
    }

    loadAccount() {
        firstValueFrom(this.apiClient.getAccount(this.accountId))
            .then(a => {
                this.resource = {
                    state: 'success',
                    value: a
                };

                this.accountForm.patchValue({
                    name: a.name
                });
            })
            .catch((e: ChangeBlogManagementApi.SwaggerException) => {
                if (e.status === 404)
                    this.resource = {state: 'not-found'};
                else
                    this.resource = {state: 'error', errorDetails: e.result?.errors ?? []};
            })
    }

    private async handleError(error: ChangeBlogManagementApi.SwaggerException) {
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
