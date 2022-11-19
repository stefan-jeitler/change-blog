import {Component, Input, OnInit} from '@angular/core';
import {TranslationKey} from "../../../generated/TranslationKey";
import {ChangeBlogManagementApi} from "../../../../clients/ChangeBlogManagementApiClient";
import {TranslocoService} from "@ngneat/transloco";
import {ConfirmationService} from "primeng/api";
import {firstValueFrom} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-account-delete',
  templateUrl: './account-delete.component.html',
  styleUrls: ['./account-delete.component.scss']
})
export class AccountDeleteComponent implements OnInit {

  @Input() accountId: string;
  @Input() accountName: string;
  errorMessage: string;
  fatalError: boolean;

  constructor(public translationKey: TranslationKey,
              private translationService: TranslocoService,
              private apiClient: ChangeBlogManagementApi.Client,
              private confirmationService: ConfirmationService,
              private router: Router) {
    this.accountId = '';
    this.accountName = '';
    this.errorMessage = '';
    this.fatalError = false;
  }

  ngOnInit(): void {
  }

  async askForDeletion() {
    const title = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.confirm));
    let confirmationMessage = this.translationService
      .selectTranslate(this.translationKey.confirmAccountDeletion,
        {accountName: this.accountName});

    this.confirmationService.confirm({
      message: await firstValueFrom(confirmationMessage),
      header: title,
      icon: 'pi pi-exclamation-triangle',
      accept: () => this.deleteAccount()
    });
  }

  deleteAccount() {
    firstValueFrom(this.apiClient.deleteAccount(this.accountId))
      .then(() => this.router.navigateByUrl('/app/accounts'))
      .catch((e: ChangeBlogManagementApi.SwaggerException) => {
        const error = e.result?.errors?.shift()?.messages?.shift();

        if (error)
          this.errorMessage = error;
        else
          this.fatalError = true;
      });
  }

}
