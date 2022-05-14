import {Component, Inject, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {ConfirmationService, MessageService} from "primeng/api";
import {ChangeBlogManagementApi} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {Clipboard} from '@angular/cdk/clipboard';
import {TranslocoService} from "@ngneat/transloco";
import {AppConfig} from "../../../../app.config";
import {ChangeBlogApi} from "../../../clients/ChangeBlogApiClient";

interface ApiKey {
  id: string,
  title: string,
  key: string,
  expires: Date
}

@Component({
  selector: 'app-apikey',
  templateUrl: './apikey.component.html',
  styleUrls: ['./apikey.component.scss']
})
export class ApikeyComponent implements OnInit {
  selectedApiKeys: ApiKey[];
  apiKeys: ApiKey[];
  isLoadingFinished: boolean;
  showApiKeyDialog: boolean;
  apiKeyForm: FormGroup;
  maxApiKeysCount: number = 5;
  minExpires: Date;
  maxExpires: Date;

  constructor(public translationKey: TranslationKey,
              private messageService: MessageService,
              private mngmtApiClient: ChangeBlogManagementApi.Client,
              private formBuilder: FormBuilder,
              private confirmationService: ConfirmationService,
              private clipboard: Clipboard,
              private translationService: TranslocoService,
              @Inject(ChangeBlogApi.API_BASE_URL) public apiBaseUrl: string) {

    this.isLoadingFinished = false;
    this.showApiKeyDialog = false;

    this.apiKeyForm = this.formBuilder.group({
      id: new FormControl(''),
      title: new FormControl(''),
      expires: new FormControl('')
    });

    this.selectedApiKeys = [];
    this.apiKeys = [];

    this.minExpires = new Date(new Date().getTime()+(8*24*60*60*1000));
    const twoYearsAhead = new Date();
    twoYearsAhead.setFullYear(twoYearsAhead.getFullYear() + 2)
    this.maxExpires = twoYearsAhead;
  }

  async ngOnInit() {
    await this.loadApiKeys();
    this.isLoadingFinished = true;
  }

  async createNewApiKey() {
    this.apiKeyForm.reset();
    this.showApiKeyDialog = true;
  }

  async deleteSelectedApiKeys() {

    const deleteKeys = async () => {
      for (const x of this.selectedApiKeys) {
        await firstValueFrom(this.mngmtApiClient.deleteApiKey(x.id));
      }

      await this.loadApiKeys();
    };

    const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
    const confirmationQuestion = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirmSelecetedApiKeysDeletion));
    const success = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.success));
    const apiKeysDeletedMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.apiKeyDeleted));
    this.confirmationService.confirm({
      message: confirmationQuestion,
      header: title,
      icon: 'pi pi-exclamation-triangle',
      accept: async () => {
        await deleteKeys();
        this.messageService.add({severity: 'success', summary: success, detail: apiKeysDeletedMessage, life: 3000});
      }
    });
  }

  updateApiKey(apiKey: ApiKey) {
    this.apiKeyForm.patchValue({
      id: apiKey.id,
      title: apiKey.title,
      expires: apiKey.expires
    });

    this.showApiKeyDialog = true;
  }

  async loadApiKeys() {
    const apiKeys = await firstValueFrom(this.mngmtApiClient.getApiKeys());
    this.apiKeys = apiKeys.map(x => {
      return {
        id: x.apiKeyId,
        title: x.title!,
        key: x.apiKey!,
        expires: x.expiresAt
      }
    });

  }

  async deleteApiKey(apiKey: ApiKey) {
    const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
    const confirmationQuestion = await firstValueFrom(this.translationService.selectTranslate(
      this.translationKey.confirmApiKeyDeletion,
      {apiKeyTitle: apiKey.title}));

    const success = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.success));
    const apiKeysDeletedMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.apiKeyDeleted));
    this.confirmationService.confirm({
      message: confirmationQuestion,
      header: title,
      icon: 'pi pi-exclamation-triangle',
      accept: async () => {
        await firstValueFrom(this.mngmtApiClient.deleteApiKey(apiKey.id));
        this.messageService.add({severity: 'success', summary: success, detail: apiKeysDeletedMessage, life: 3000});
        await this.loadApiKeys();
      }
    });
  }

  closeApiKeyDialog() {
    this.showApiKeyDialog = false;
  }

  async onApiKeySubmit(apiKeyForm: FormGroup) {
    const title = this.apiKeyForm.value.title;
    const expires = this.apiKeyForm.value.expires

    if(!!apiKeyForm.value.id)
      await firstValueFrom(this.mngmtApiClient.updateApiKey(apiKeyForm.value.id, title, expires))
    else
      await firstValueFrom(this.mngmtApiClient.generateApiKey(title, expires))

    this.showApiKeyDialog = false;
    await this.loadApiKeys();
  }

  async copyToClipBoard(apiKey: ApiKey) {
    this.clipboard.copy(apiKey.key);

    const summary = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.info));
    let messageTranslation = this.translationService.selectTranslate(this.translationKey.ApiKeyCopiedToClipboard);
    const message = await firstValueFrom(messageTranslation);
    this.messageService.add({
      severity: 'success',
      summary: summary,
      detail: message,
      life: 4000
    });
  }
}
