import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {tap} from "rxjs/operators";
import {ChangeBlogManagementApi as MngmtApiClient} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {Message, MessageService} from "primeng/api";
import {TranslocoService} from "@ngneat/transloco";
import {AppCultureService} from "../../services/app-culture.service";
import ITimezoneDto = MngmtApiClient.ITimezoneDto;

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  availableTimezones: ITimezoneDto[];
  availableCultures: string[];
  isLoadingFinished: boolean;
  userProfileForm: FormGroup;
  messages: Message[];
  messageHidingTimeout: NodeJS.Timeout | undefined;

  constructor(public translationKey: TranslationKey,
              private translationService: TranslocoService,
              private appCultureService: AppCultureService,
              private messageService: MessageService,
              private mngmtApiClient: MngmtApiClient.Client,
              private formBuilder: FormBuilder) {

    this.availableTimezones = [];
    this.availableCultures = [];

    this.messages = [];

    this.isLoadingFinished = false;

    this.userProfileForm = this.formBuilder.group({
      fullName: new FormControl({value: null, disabled: true}),
      email: new FormControl({value: null, disabled: true}),
      timezone: new FormControl(''),
      culture: new FormControl('')
    });
  }

  async ngOnInit(): Promise<void> {

    await this.loadAvailableDropdownOptions();
    const userProfile = await this.loadUserProfile();

    this.userProfileForm.patchValue({
      fullName: `${userProfile.firstName} ${userProfile.lastName}`,
      email: userProfile.email,
      timezone: this.availableTimezones.find(x => x.olsonId === userProfile.timeZone),
      culture: userProfile.culture
    });
  }

  enableForm() {
    for (const controlKey of ['timezone', 'culture']) {
      this.userProfileForm.controls[controlKey].enable();
    }
  }

  disableForm() {
    for (const controlKey of ['timezone', 'culture']) {
      this.userProfileForm.controls[controlKey].disable();
    }
  }

  updateProfile(userProfileForm: FormGroup) {
    userProfileForm.resetValidation();
    this.disableForm();

    const dto = new MngmtApiClient.UpdateUserProfileDto()
    dto.culture = userProfileForm.value.culture;
    dto.timezone = userProfileForm.value.timezone.olsonId;

    this.mngmtApiClient.updateUserProfile(dto)
      .subscribe({
        next: r => this.profileUpdated(r),
        error: async (error: MngmtApiClient.SwaggerException) => {
          await this.handleError(error);
        },
        complete: () => this.enableForm()
      });
  }

  private async handleError(error: MngmtApiClient.SwaggerException) {
    this.enableForm();

    if (error.status >= 400 && error.status < 500)
      this.userProfileForm.setServerError(error.result.errors);
    else {
      await this.showGenericErrorMessage();
    }
  }

  private async showGenericErrorMessage() {
    const errorMessageHeader = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.genericErrorMessageShort));
    const errorMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.genericErrorMessage));

    const message = {severity: 'error', summary: errorMessageHeader, detail: errorMessage}
    this.messageService.add(message);
  }

  private async loadUserProfile() {
    const loadUserProfile = this.mngmtApiClient
      .getUserProfile()
      .pipe(tap(_ => this.isLoadingFinished = true))

    return await firstValueFrom(loadUserProfile);
  }

  private async loadAvailableDropdownOptions() {
    const loadTimezones = this.mngmtApiClient
      .getSupportedTimezones();
    this.availableTimezones = await firstValueFrom(loadTimezones);

    const loadCulture = this.mngmtApiClient
      .getSupportedCultures()
    this.availableCultures = await firstValueFrom(loadCulture);
  }

  private async profileUpdated(response: MngmtApiClient.SuccessResponse) {
    await this.appCultureService.applyUserCulture();

    const userProfileUpdateMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.userProfileUpdated));

    if (!!this.messageHidingTimeout)
      clearTimeout(this.messageHidingTimeout);

    this.messages = [{severity: 'success', summary: '', detail: userProfileUpdateMessage}];
    this.messageHidingTimeout = setTimeout(() => this.messages = [], 4000);
  }
}
