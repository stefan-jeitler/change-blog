import {Component, Input, OnInit} from '@angular/core';
import {ChangeBlogManagementApi} from "../../../../clients/ChangeBlogManagementApiClient";
import {TranslationKey} from "../../../generated/TranslationKey";
import {translate} from "@ngneat/transloco";
import ErrorMessages = ChangeBlogManagementApi.ErrorMessages;

@Component({
  selector: 'app-content-error',
  templateUrl: './content-error.component.html',
  styleUrls: ['./content-error.component.scss']
})
export class ContentErrorComponent implements OnInit {

  @Input() details: ErrorMessages[];

  constructor(public translationKey: TranslationKey) {
    this.details = []
  }

  get errors(): string[] {
    const errors = this.details
      .flatMap(x => x.messages ?? '')
      .filter(x => !!x);

    return errors.length !== 0
      ? errors
      : [translate(this.translationKey.unexpectedError)]
  }

  ngOnInit(): void {
  }

}
