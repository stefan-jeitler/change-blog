export interface ServerError {
  message: string;
}

export class ValidationError {
  constructor(public serverError: ServerError) {
  }
}
