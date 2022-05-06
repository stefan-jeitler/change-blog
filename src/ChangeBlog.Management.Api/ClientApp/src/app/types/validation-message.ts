export interface ServerError {
  message: string;
}

export class ValidationMessage {
  constructor(public serverError: ServerError) {
  }
}
