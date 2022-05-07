import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {tap} from "rxjs/operators";
import {
  ChangeBlogManagementApi,
  ChangeBlogManagementApi as MngmtApiClient
} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {MessageService} from "primeng/api";
import {TranslocoService} from "@ngneat/transloco";
import {TranslocoLocaleService} from "@ngneat/transloco-locale";
import ITimezoneDto = MngmtApiClient.ITimezoneDto;
import SwaggerException = ChangeBlogManagementApi.SwaggerException;


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

  constructor(public translationKey: TranslationKey,
              private translationService: TranslocoService,
              private localService: TranslocoLocaleService,
              private messageService: MessageService,
              private mngmtApiClient: MngmtApiClient.Client,
              private formBuilder: FormBuilder) {

    this.availableTimezones = [];
    this.availableCultures = [];

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
    })
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

  async updateProfile(userProfileForm: FormGroup) {
    userProfileForm.resetValidation();
    this.disableForm()

    const dto = new MngmtApiClient.UpdateUserProfileDto()
    dto.culture = userProfileForm.value.culture;
    dto.timezone = userProfileForm.value.timezone.olsonId;

    this.mngmtApiClient.updateUserProfile(dto)
      .subscribe({
        next: r => this.profileUpdated(r),
        error: (error: SwaggerException) => {
          this.handleError(error);
        },
        complete: () => this.enableForm()
      });
  }

  private async handleError(error: ChangeBlogManagementApi.SwaggerException) {
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
    loadTimezones.subscribe(t => this.availableTimezones = t);

    const loadCulture = this.mngmtApiClient
      .getSupportedCultures()
    loadCulture.subscribe(c => this.availableCultures = c);

    await Promise.all([firstValueFrom(loadTimezones), firstValueFrom(loadCulture)]);
  }

  private async profileUpdated(response: ChangeBlogManagementApi.SuccessResponse) {
    const successMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.success));

    const profile = await firstValueFrom(this.mngmtApiClient
      .getUserCulture());

    this.localService.setLocale(profile.culture!);
    this.translationService.setActiveLang(profile.language!);
    await firstValueFrom(this.translationService.load(profile.language!));

    const message = {severity: 'success', summary: successMessage, detail: response.message}
    this.messageService.add(message);
  }
}
