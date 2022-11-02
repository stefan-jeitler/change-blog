import {Component, Inject, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {ConfirmationService, MenuItem, MessageService} from "primeng/api";
import {
    ChangeBlogManagementApi as MngmtApiClient,
    ChangeBlogManagementApi,
} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {FormControl, UntypedFormBuilder, UntypedFormGroup} from "@angular/forms";
import {Clipboard} from '@angular/cdk/clipboard';
import {translate, TranslocoService} from "@ngneat/transloco";
import "../../extensions/message-service.extensions";
import {ChangeBlogApi} from "../../../clients/ChangeBlogApiClient";
import ApiKeyDto = ChangeBlogManagementApi.CreateOrUpdateApiKeyDto;

interface ApiKey {
    id: string,
    name: string,
    key: string,
    expiresAt: Date
}

@Component({
    selector: 'app-apikey',
    templateUrl: './apikey.component.html',
    styleUrls: ['./apikey.component.scss']
})
export class ApikeyComponent implements OnInit {
    selectedApiKeys: ApiKey[];
    apiKeys: ApiKey[];
    actionMenuTarget: ApiKey | undefined;
    isLoadingFinished: boolean;
    showApiKeyDialog: boolean;
    apiKeyForm: UntypedFormGroup;
    readonly maxApiKeysCountLimit: number = 5;
    minExpires: Date;
    maxExpires: Date;
    contextMenuItems: MenuItem[];
    showDatatableLoadingOverlay: boolean;

    constructor(public translationKey: TranslationKey,
                private messageService: MessageService,
                private mngmtApiClient: ChangeBlogManagementApi.Client,
                private formBuilder: UntypedFormBuilder,
                private confirmationService: ConfirmationService,
                private clipboard: Clipboard,
                private translationService: TranslocoService,
                @Inject(ChangeBlogApi.API_BASE_URL) public apiBaseUrl: string) {

        this.isLoadingFinished = false;
        this.showApiKeyDialog = false;
        this.showDatatableLoadingOverlay = false;

        this.apiKeyForm = this.formBuilder.group({
            id: new FormControl<string>(''),
            name: new FormControl<string>(''),
            expiresAt: new FormControl<string | undefined>(undefined)
        });

        this.contextMenuItems = [
            {
                label: translate(this.translationKey.copyToClipboard),
                command: async () => {
                    if (!!this.actionMenuTarget)
                        await this.copyToClipBoard(this.actionMenuTarget);
                },
                icon: 'pi pi-fw pi-copy'
            },
            {
                label: translate(this.translationKey.edit),
                command: async () => {
                    if (!!this.actionMenuTarget)
                        await this.updateApiKey(this.actionMenuTarget);
                },
                icon: 'pi pi-fw pi-pencil'
            },
            {
                label: translate(this.translationKey.delete),
                command: async () => {
                    if (!!this.actionMenuTarget)
                        await this.deleteApiKey(this.actionMenuTarget);
                },
                icon: 'pi pi-fw pi-trash'
            }
        ];

        this.selectedApiKeys = [];
        this.apiKeys = [];

        this.minExpires = new Date(new Date().getTime() + (5 * 24 * 60 * 60 * 1000));
        const twoYearsAhead = new Date();
        twoYearsAhead.setFullYear(twoYearsAhead.getFullYear() + 2)
        this.maxExpires = twoYearsAhead;
    }

    async ngOnInit() {
        try {
            await this.loadApiKeys();
        } finally {
            this.isLoadingFinished = true;
        }
    }

    async createNewApiKey() {
        this.apiKeyForm.reset();
        this.apiKeyForm.resetValidation()
        this.showApiKeyDialog = true;
    }

    async deleteSelectedApiKeys() {
        const deleteKeys = async () => {
            this.showDatatableLoadingOverlay = true;

            for (const x of this.selectedApiKeys) {
                await firstValueFrom(this.mngmtApiClient.deleteApiKey(x.id));
            }

            await this.loadApiKeys();
            this.selectedApiKeys = [];
            this.showDatatableLoadingOverlay = false;
        };

        const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
        const confirmationQuestion = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirmSelectedApiKeysDeletion));
        this.confirmationService.confirm({
            message: confirmationQuestion,
            header: title,
            icon: 'pi pi-exclamation-triangle',
            accept: async () => {
                await deleteKeys();
            }
        });
    }

    updateApiKey(apiKey: ApiKey) {
        this.apiKeyForm.patchValue({
            id: apiKey.id,
            name: apiKey.name,
            expiresAt: apiKey.expiresAt
        });

        this.showApiKeyDialog = true;
    }

    async loadApiKeys() {
        const apiKeys = await firstValueFrom(this.mngmtApiClient.getApiKeys());
        this.apiKeys = apiKeys.map(x => {
            return {
                id: x.apiKeyId,
                name: x.name!,
                key: x.apiKey!,
                expiresAt: x.expiresAt
            }
        });
    }

    async deleteApiKey(apiKey: ApiKey) {
        const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
        const confirmationQuestion = await firstValueFrom(this.translationService.selectTranslate(
            apiKey.name
                ? this.translationKey.confirmApiKeyDeletion
                : this.translationKey.confirmApiKeyDeletionWithoutApiKeyName,
            {apiKeyName: apiKey.name}));

        this.confirmationService.confirm({
            message: confirmationQuestion,
            header: title,
            icon: 'pi pi-exclamation-triangle',
            accept: async () => {
                this.showDatatableLoadingOverlay = true;
                await firstValueFrom(this.mngmtApiClient.deleteApiKey(apiKey.id));
                await this.loadApiKeys();
                this.showDatatableLoadingOverlay = false;
            }
        });
    }

    closeApiKeyDialog() {
        this.showApiKeyDialog = false;
    }

    async onApiKeySubmit() {
        this.apiKeyForm.disable();

        let apiKeyId = this.apiKeyForm.value.id;

        const dto = ApiKeyDto.fromJS({
            name: this.apiKeyForm.value.name,
            expiresAt: this.apiKeyForm.value.expiresAt
        })

        const generateOrUpdateRequest = !!apiKeyId
            ? this.mngmtApiClient.updateApiKey(apiKeyId, undefined, dto)
            : this.mngmtApiClient.generateApiKey(undefined, dto);

        generateOrUpdateRequest
            .subscribe({
                next: async () => {
                    this.showDatatableLoadingOverlay = true;
                    this.apiKeyForm.enable();
                    this.showApiKeyDialog = false;
                    await this.loadApiKeys();
                    this.showDatatableLoadingOverlay = false;
                },
                error: (error: MngmtApiClient.SwaggerException) => {
                    this.apiKeyForm.enable();
                    this.handleError(error);
                }
            });
    }

    async copyToClipBoard(apiKey: ApiKey) {
        this.clipboard.copy(apiKey.key);

        let copiedMessage = this.translationService.selectTranslate(this.translationKey.apiKeyCopiedToClipboard);
        this.messageService.add({
            severity: 'info',
            detail: await firstValueFrom(copiedMessage),
            life: 4000
        });
    }

    private async handleError(error: MngmtApiClient.SwaggerException) {
        this.apiKeyForm.enable();

        if (error.status >= 400 && error.status < 500) {
            this.apiKeyForm.setErrors(error.result.errors);
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
