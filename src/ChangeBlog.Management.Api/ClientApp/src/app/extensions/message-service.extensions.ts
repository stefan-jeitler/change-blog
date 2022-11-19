import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import {MessageService} from "primeng/api";
import ErrorMessage = ChangeBlogManagementApi.ErrorMessages;


declare module "primeng/api/messageservice" {
  interface MessageService {
    showGeneralErrors: (errorMessages: ErrorMessage[]) => void
  }
}

MessageService.prototype.showGeneralErrors = function (this: MessageService, errorMessages: ErrorMessage[]): void {
  const generalErrorMessages = errorMessages.filter((x: ErrorMessage) => !x.property);

  for (const x of generalErrorMessages) {
    const msg = x.messages?.reduce((acc: string, next: string) => acc ? `${acc}\n${next}` : next, '\n') ?? 'Unknown Error';
    this.add({severity: 'error', detail: msg, life: 7000});
  }
}
