import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {ChangeBlogManagementApi as MngmtApiClient} from "../../../clients/ChangeBlogManagementApiClient";
import {firstValueFrom} from "rxjs";
import {FormControl, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup} from "@angular/forms";
import {MessageService} from "primeng/api";
import {TranslocoService} from "@ngneat/transloco";
import {AppUserService} from "../../services/app-user.service";
import "../../extensions/message-service.extensions";
import {Resource} from "../resource.state";
import ITimezoneDto = MngmtApiClient.ITimezoneDto;
import UserDto = MngmtApiClient.UserDto;

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    availableTimezones: ITimezoneDto[];
    availableCultures: string[];
    userProfileForm: UntypedFormGroup;
    resource: Resource<UserDto>;

    constructor(public translationKey: TranslationKey,
                private translationService: TranslocoService,
                private appCultureService: AppUserService,
                private messageService: MessageService,
                private mngmtApiClient: MngmtApiClient.Client,
                private formBuilder: UntypedFormBuilder) {

        this.availableTimezones = [];
        this.availableCultures = [];

        this.resource = {state: 'loading'};

        this.userProfileForm = this.formBuilder.group({
            fullName: new UntypedFormControl({value: null, disabled: true}),
            email: new UntypedFormControl({value: null, disabled: true}),
            timezone: new FormControl<string>(''),
            culture: new FormControl<string>('')
        });
    }

    async ngOnInit(): Promise<void> {
        try {
            await this.loadAvailableDropdownOptions();
            this.loadUserProfile()
                .then(up => {
                    this.resource = {
                        state: 'loaded',
                        value: up
                    };

                    this.userProfileForm.patchValue({
                        fullName: `${up.firstName} ${up.lastName}`,
                        email: up.email,
                        timezone: this.availableTimezones.find(x => x.olsonId === up.timeZone),
                        culture: up.culture
                    });
                })
                .catch((e: MngmtApiClient.SwaggerException) => {
                    this.resource = {
                        state: 'error',
                        errorDetails: e.result?.errors ?? []
                    };
                });
        } catch (error: any) {
            this.resource = {
                state: 'error',
                errorDetails: error?.result?.errors ?? []
            }
        }
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

    updateProfile(userProfileForm: UntypedFormGroup) {
        userProfileForm.resetValidation();
        this.disableForm();

        const dto = new MngmtApiClient.UpdateUserProfileDto()
        dto.culture = userProfileForm.value.culture;
        dto.timezone = userProfileForm.value.timezone.olsonId;

        this.mngmtApiClient.updateUserProfile(undefined, dto)
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

        if (error.status >= 400 && error.status < 500) {
            this.userProfileForm.setErrors(error.result.errors);
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

    private async loadUserProfile() {
        const loadUserProfile = this.mngmtApiClient
            .getUserProfile();

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
        await this.appCultureService.applyUserSettings();
        this.userProfileForm.markAsPristine();
        this.userProfileForm.markAsUntouched();

        const userProfileUpdateMessage = await firstValueFrom(this.translationService.selectTranslate(this.translationKey.userProfileUpdated));

        this.messageService.add({
            severity: 'success',
            detail: userProfileUpdateMessage
        });
    }
}
